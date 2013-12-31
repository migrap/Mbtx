using Mbtx.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mbtx.Net {
    internal class ConnectSettings {
        public IPEndPoint EndPoint { get; set; }
        public int ReceiveBufferSize { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal class Message {
        public byte[] Buffer;
        public int Count;
    }

    public class QuoteClient {
        private const string SubscribeMessage = "S|1003={0};2000=20000\n";
        private const string LogonMessage = "L|100={0};101={1}\n";
        private const int Port = 5020;
        private ConcurrentDictionary<string, Ticker> _tickers = new ConcurrentDictionary<string, Ticker>();
        private SocketClient _socket;
        private string _username;
        private string _password;
                
        private Subject<Ticker> _ticker = new Subject<Ticker>();
        private ConcurrentDictionary<string, IObservable<Ticker>> _subscriptions = new ConcurrentDictionary<string, IObservable<Ticker>>();
        private TaskCompletionSource<bool> _connected;
        private Message _message;
        private Subject<byte[]> _packets = new Subject<byte[]>();        
                
        public QuoteClient() {
            _packets.Subscribe(OnPacket);            
        }

        public Task<bool> ConnectAsync(string username, string password) {
            var address = (IPAddress)null;
            var port = Port;

            _username = username;
            _password = password;

            _connected = new TaskCompletionSource<bool>();

            try {
                address = GetQuoteServerAsync(username, password).Result;                
            } catch(Exception ex) {
                _connected.SetException(ex);
            }

            _socket = new SocketClient(address, port);
            _socket.Received += OnReceive;
            _socket.Connected += OnConnected;
            _socket.Disconnected += OnDisconnected;

            _socket.Connect();

            return _connected.Task;
        }

        private void OnReceive(object sender, SocketAsyncEventArgs e) {
            _message = _message ?? (_message = new Message() { Buffer = e.Buffer });

            if(e.SocketError == SocketError.Success && e.BytesTransferred > 0) {
                var buffer = e.Buffer;
                var count = e.BytesTransferred;
                var message = _message;

                if(count <= 0) {
                    return;
                }

                message.Count += count;
                if(message.Count > buffer.Length) {
                    return;
                }

                for(int i = message.Count - 1; i >= 1; i--) {
                    if(buffer[i] == 0x0a) {
                        count = i + 1;
                        message.Count = message.Count - count;
                        break;
                    }
                }

                var packet = new byte[count];
                Buffer.BlockCopy(buffer, 0, packet, 0, count);
                _packets.OnNext(packet);

                if(message.Count != 0) {
                    Buffer.BlockCopy(message.Buffer, count, message.Buffer, 0, message.Count);
                }
                _socket.Receive(message.Count, message.Buffer.Length - message.Count);

            } else {
                OnDisconnected(sender, e);
            }
        }

        private void OnConnected(object sender, SocketAsyncEventArgs e) {
            _socket.Receive();            
            _socket.Send("L|100={0};101={1}\n".FormatWith(_username, _password));
        }

        private void OnDisconnected(object sender, SocketAsyncEventArgs e) {
            var address = (IPAddress)null;
            var port = Port;

            try {
                address = GetQuoteServerAsync(_username, _password).Result;
                _socket.RemoteEndPoint = new IPEndPoint(address, port);

            } finally {
                _socket.Connect();
            }
        }

        private void OnPacket(byte[] buffer) {
            var message = Encoding.ASCII.GetString(buffer).Replace("\n", String.Empty);

            switch (message[0]) {
                case 'G':
                    ProcessLogonAccepted(message);
                    break;
                case 'D':
                    ProcessLogonDenied(message);
                    break;
                case '1':
                    ProcessLevel1Update(message);
                    break;
                default:
                    break;
            }
        }

        private void ProcessLogonAccepted(string message) {
            _connected.SetResult(true);
            _subscriptions.Keys.ToArray().Foreach(x => Subscribe(x));
        }

        private void ProcessLogonDenied(string message) {
            message = message.GetLogonDeniedReason();
            _connected.SetException(new InvalidCredentialException(message));
        }

        private void ProcessLevel1Update(string message) {
            var lines = message.Split(new string[] { "1|" }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < lines.Length; i++) {

                var split = lines[i].Split("|;".ToCharArray());

                var symbol = split.Where(s => s.StartsWith("1003=")).Select(s => s.Replace("1003=", string.Empty)).FirstOrDefault();

                var ticker = _tickers.GetOrAdd(symbol, (s) => new Ticker() { Symbol = s });

                for(int j = 0; j < split.Length; j++) {
                    var tev = split[j].Split("=".ToCharArray(), StringSplitOptions.None);
                    if(tev.Length != 2) { continue; }

                    var date = DateTime.Now.Date;
                    var time = DateTime.Now.TimeOfDay;

                    switch(tev[0]) {
                        case "2003":
                            ticker.BidPrice = double.Parse(tev[1]);
                            break;
                        case "2004":
                            ticker.AskPrice = double.Parse(tev[1]);
                            break;
                        case "2002":
                            ticker.LastPrice = double.Parse(tev[1]);
                            break;
                        case "2007":
                            ticker.LastSize = int.Parse(tev[1]);
                            break;
                        case "2005":
                            ticker.BidSize = int.Parse(tev[1]);
                            break;
                        case "2006":
                            ticker.AskSize = int.Parse(tev[1]);
                            break;
                        case "2015":
                            date = DateTime.ParseExact(tev[1], "MM/dd/yyyy", null);
                            break;
                        case "2014":
                            time = TimeSpan.Parse(tev[1]);
                            break;
                        default:
                            break;
                    }

                    date = date.Date.Add(time);
                    ticker.DateTime = new DateTimeOffset(date);
                }

                _ticker.OnNext(ticker);
            }
        }

        public IObservable<Ticker> Subscribe(string symbol) {
            _socket.Send(SubscribeMessage.FormatWith(symbol));
            return _subscriptions.GetOrAdd(symbol, (s) => _ticker.Where(q => q.Symbol == symbol));
        }

        private async Task<IPAddress> GetQuoteServerAsync(string username, string password) {
            var builder = new UriBuilder { Scheme = "https", Host = "www.mbtrading.com", Path = "secure/getquoteserverxml.aspx", Query = "username={0}&password={1}".FormatWith(username, password) };
            var http = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
            var content = await http.SendAsync(request).Result.Content.ReadAsStringAsync();
            var attribute = XDocument.Parse(content)
                .Descendants("logins").First()
                .Attributes()
                .Where(xa => xa.Name.LocalName.Equals("quote_server", StringComparison.InvariantCultureIgnoreCase)).First();
            return IPAddress.Parse(attribute.Value);
        }
    }

    public static partial class Extensions {
        internal static void Send(this SocketClient socket, string value) {
            Send(socket, value, Encoding.ASCII);
        }

        internal static void Send(this SocketClient socket, string value, Encoding encoding) {
            var buffer = encoding.GetBytes(value);
            socket.Send(buffer, 0, buffer.Length);
        }
    }
}