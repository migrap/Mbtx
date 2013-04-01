using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mbtx.Net {
    public class QuoteClient {
        private const string SubscribeMessage = "S|1003={0};2000=20000\n";
        private const string LogonMessage = "L|100={0};101={1}\n";
        private ConcurrentDictionary<string, Quote> _quotes = new ConcurrentDictionary<string, Quote>();

        private AutoResetEvent _logonAutoResetEvent = new AutoResetEvent(false);
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private SocketAsyncEventArgs _receive = new SocketAsyncEventArgs();        
        private Encoding _encdoing = Encoding.ASCII;
        private Subject<Quote> _subject = new Subject<Quote>();
        private ConcurrentDictionary<string, IObservable<Quote>> _subjects = new ConcurrentDictionary<string, IObservable<Quote>>();
        
        public QuoteClient() {                      
        }

        public void Connect(string username, string password) {
            ConnectAsync(username, password).Wait();
        }

        public async Task ConnectAsync(string username, string password) {
            var address = await GetQuoteServerAsync(username, password);
            var port = 5020;
            await ConnectAsync(address, port, username, password);
        }        

        public async Task ConnectAsync(IPAddress address, int port, string username, string password) {
            await Task.Run(() => {
                _socket.Connect(address, port);
                _socket.Receive(OnReceive, OnDisconnect);
                _socket.Send("L|100={0};101={1}\n".FormatWith(username, password));
                _logonAutoResetEvent.WaitOne(TimeSpan.FromSeconds(10));
            });
        }

        public IObservable<Quote> Subscribe(string symbol) {
            _socket.Send(SubscribeMessage.FormatWith(symbol));
            return _subjects.GetOrAdd(symbol, (s) => _subject.Where(q => q.Symbol == symbol));
        }

        private void OnReceive(SocketAsyncEventArgs e) {
            var bytes = e.GetBytesTransfered();
            var message = Encoding.ASCII.GetString(bytes);

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

            if (_socket.Connected) {
                _socket.ReceiveAsync(e);
            }
        }

        private void OnDisconnect(SocketAsyncEventArgs e) {
        }

        private void ProcessLogonAccepted(string message) {
            _logonAutoResetEvent.Set();
        }

        private void ProcessLogonDenied(string message) {
            var reason = message.GetLogonDeniedReason();
            throw new LogonException(reason);
        }

        private void ProcessLevel1Update(string message) {
            if ((message.IsNullOrEmpty()) || (false == message.Contains("1003="))) { return; }
            try {
                var lines = message.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var quotes = new List<Quote>();                

                for (int i = 0; i < lines.Length; i++) {

                    var split = lines[i].Split("|;".ToCharArray());

                    var symbol = split.Where(s => s.StartsWith("1003=")).Select(s => s.Replace("1003=", string.Empty)).FirstOrDefault();

                    var quote = _quotes.GetOrAdd(symbol, (s) => new Quote() { Symbol = s });

                    for (int j = 0; j < split.Length; j++) {
                        var tev = split[j].Split("=".ToCharArray(), StringSplitOptions.None);
                        if (tev.Length != 2) { continue; }

                        var date = DateTime.Now.Date;
                        var time = DateTime.Now.TimeOfDay;

                        switch (tev[0]) {
                            case "2003": quote.BidPrice = double.Parse(tev[1]); break;
                            case "2004": quote.AskPrice = double.Parse(tev[1]); break;
                            case "2002": quote.LastPrice = double.Parse(tev[1]); break;
                            case "2007": quote.LastSize = int.Parse(tev[1]); break;
                            case "2005": quote.BidSize = int.Parse(tev[1]); break;
                            case "2006": quote.AskSize = int.Parse(tev[1]); break;
                            case "2015": date = DateTime.ParseExact(tev[1], "MM/dd/yyyy", null); break;
                            case "2014": time = TimeSpan.Parse(tev[1]); break;
                            default: break;
                        }

                        date = date.Date.Add(time);
                        quote.DateTime = new DateTimeOffset(date);
                    }

                    quotes.Add(quote);                    
                }

                if (quotes.Count != 1) {
                    bool here = true;
                }
                quotes.Distinct().Foreach(OnNext);
                
            }
            catch (Exception ex) {
                Console.WriteLine("{0} -> {1}", ex.Message, message);
            }
        }

        private void OnNext(Quote quote) {
            _subject.OnNext(quote);
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
}