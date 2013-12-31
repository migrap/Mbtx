using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mbtx.Net {
    internal class SocketClient {
        private Socket _socket;
        private SocketAsyncEventArgs _receive;
        private EndPoint _endPoint;
        private TimeSpan _reconnect;

        public event EventHandler<SocketAsyncEventArgs> Connected;

        public event EventHandler<SocketAsyncEventArgs> Received;

        public event EventHandler<SocketAsyncEventArgs> Sent;

        public event EventHandler<EventArgs> Closed;

        public event EventHandler<SocketAsyncEventArgs> Disconnected;

        public event EventHandler<ExceptionEventArgs> Error;

        public event EventHandler<SocketAsyncEventArgs> Completed;

        public SocketClient(string address, int port, int receiveBufferSize = 8096, int sendBufferSize = 8096, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
            : this(IPAddress.Parse(address), port, receiveBufferSize, sendBufferSize, addressFamily, socketType, protocolType) {
        }

        public SocketClient(Uri uri, int receiveBufferSize = 8096, int sendBufferSize = 8096, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
            : this(uri.Host, uri.Port, receiveBufferSize, sendBufferSize) {
        }

        public SocketClient(Socket socket, IPAddress address, int port, int receiveBufferSize = 8096, int sendBufferSize = 8096)
            : this(socket, new IPEndPoint(address, port), receiveBufferSize, sendBufferSize) {
        }

        public SocketClient(IPAddress address, int port, int receiveBufferSize = 8096, int sendBufferSize = 8096, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
            : this(new IPEndPoint(address, port), receiveBufferSize, sendBufferSize, addressFamily, socketType, protocolType) {
        }

        public SocketClient(IPEndPoint endPoint, int receiveBufferSize = 8096, int sendBufferSize = 8096, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
            : this(new Socket(addressFamily, socketType, protocolType), endPoint, receiveBufferSize, sendBufferSize) {
        }

        public SocketClient(DnsEndPoint endPoint, int receiveBufferSize = 8096, int sendBufferSize = 8096, AddressFamily addressFamily = AddressFamily.InterNetwork, SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp)
            : this(new Socket(addressFamily, socketType, protocolType), endPoint, receiveBufferSize, sendBufferSize) {
        }

        private SocketClient(Socket socket, EndPoint endPoint, int receiveBufferSize = 8096, int sendBufferSize = 8096)
            : this(socket, endPoint, TimeSpan.Zero, receiveBufferSize, sendBufferSize) {
        }

        private SocketClient(Socket socket, EndPoint endPoint, TimeSpan reconnect, int receiveBufferSize = 8096, int sendBufferSize = 8096) {
            _socket = socket;
            _endPoint = endPoint;
            _reconnect = reconnect;

            _receive = new SocketAsyncEventArgs();
            _receive.Completed += ProcessCompleted;
            _receive.RemoteEndPoint = _endPoint;
            _receive.UserToken = _socket;
            _receive.SetBuffer(new byte[receiveBufferSize], 0, receiveBufferSize);
        }

        public EndPoint RemoteEndPoint {
            get { return _receive.RemoteEndPoint; }
            set { _receive.RemoteEndPoint = value; }
        }

        public void Connect() {
            Console.WriteLine("Connecting to: {0}", _endPoint);
            var completed = default(EventHandler<SocketAsyncEventArgs>);

            var saea = new SocketAsyncEventArgs {
                RemoteEndPoint = _endPoint,
                UserToken = _socket
            };

            saea.Completed += completed = (s, e) => {
                e.Completed -= completed;
                if(e.SocketError == SocketError.Success) {
                    OnConnected(e);
                } else {
                    OnDisconnected(e);
                }
            };

            if(false == _socket.ConnectAsync(saea)) {
                completed(this, saea);
            }
        }        

        public void Receive() {
            _receive.SetBuffer(_receive.Buffer, 0, _receive.Buffer.Length);
            _socket.ReceiveAsync(_receive);
        }

        public void Receive(int offset, int count) {
            _receive.SetBuffer(_receive.Buffer, offset, count);
            _socket.ReceiveAsync(_receive);
        }

        public void Send(byte[] buffer, int offset = 0, int count = 0) {
            var e = new SocketAsyncEventArgs() { UserToken = _socket, RemoteEndPoint = _endPoint };
            e.SetBuffer(buffer, offset, (count == 0) ? buffer.Length : count);
            Send(e);
        }

        public void Send(SocketAsyncEventArgs e) {
            _socket.SendAsync(e);
        }

        public void Disconnect() {
            var completed = default(EventHandler<SocketAsyncEventArgs>);

            var saea = new SocketAsyncEventArgs {
                RemoteEndPoint = _endPoint,
                UserToken = _socket,
                DisconnectReuseSocket = true,
            };

            saea.Completed += completed = (s, e) => {
                e.Completed -= completed;
                ProcessCompleted(e);
            };

            if(false == _socket.DisconnectAsync(saea)) {
                completed(this, saea);
            }
        }

        private static void Disconnect(Socket socket, SocketAsyncEventArgs receive, Action<SocketAsyncEventArgs> disconnected) {
            try {
                socket.Shutdown(SocketShutdown.Both);
            } catch(Exception) {
            } finally {
                socket.Disconnect(true);
                disconnected(receive);
            }
        }

        public bool IsConnected() {
            return IsConnected(_socket);
        }

        private static bool IsConnected(Socket socket) {
            bool result = false;

            if(socket != null) {
                if(socket.Connected) {
                    result = true;
                }
            }

            return result;
        }

        #region IDisposable Members

        ~SocketClient() {
            Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        public bool Disposed {
            get { return disposed; }
        }

        protected virtual void Dispose(bool disposing) {
            if(!disposed) {
                if(disposing) {
                    //TBD: Dispose managed resources
                    Disconnect();

                    if(_receive != null) {
                        _receive.Dispose();
                        _receive = null;
                    }

                    if(_socket != null) {
                        _socket.Dispose();
                        _socket = null;
                    }
                }

                //TBD: Release unmanaged resources
            }
            disposed = true;
        }

        #endregion

        private void ProcessCompleted(object sender, SocketAsyncEventArgs e) {
            ProcessCompleted(e);
        }

        private void ProcessCompleted(SocketAsyncEventArgs e) {
            switch(e.LastOperation) {
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    ProcessDisconnect(e);
                    break;
                default:
                    OnError(new Exception("Invalid operation completed"));
                    break;
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e) {
            if(e.SocketError == SocketError.Success) {
                OnConnected(e);
            } else {
                OnError(new SocketException((int)e.SocketError));
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e) {
            if(e.SocketError == SocketError.Success && e.BytesTransferred > 0) {
                OnReceived(e);
            } else if(e.SocketError == SocketError.ConnectionReset) {
                Disconnect();
            } else {
                OnError(new SocketException((int)e.SocketError));
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e) {
            if(e.SocketError == SocketError.Success) {
                OnSent(e);
            } else {
                OnError(new SocketException((int)e.SocketError));
            }
        }

        private void ProcessDisconnect(SocketAsyncEventArgs e) {
            if(e.SocketError == SocketError.Success) {
                OnDisconnected(e);
            } else {
                OnError(new SocketException((int)e.SocketError));
            }
        }

        protected virtual void OnCompleted(SocketAsyncEventArgs value) {
            var handler = Completed;
            if(null != handler) {
                handler(this, value);
            }
        }

        protected virtual void OnConnected(SocketAsyncEventArgs value) {
            var handler = Connected;
            if(null != handler) {
                handler(this, value);
            }
        }

        protected virtual void OnDisconnected(SocketAsyncEventArgs value) {
            var handler = Disconnected;
            if(null != handler) {
                handler(this, value);
            }
        }

        protected virtual void OnSent(SocketAsyncEventArgs value) {
            var handler = Sent;
            if(null != handler) {
                handler(this, value);
            }
        }

        protected virtual void OnReceived(SocketAsyncEventArgs value) {
            var handler = Received;
            if(null != handler) {
                handler(this, value);
            }
        }

        protected virtual void OnClosed() {
            var handler = Closed;
            if(null != handler) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnError(Exception value) {
            var handler = Error;
            if(null != handler) {
                handler(this, value);
            }
        }

        private static TResult Configure<TSource, TResult>(Action<TSource> configure) where TResult : TSource, new() {
            var result = new TResult();
            configure(result);
            return result;
        }

        private static void Main(string[] args) {
            var client = new SocketClient("127.0.0.1", 1234);
            client.Received += (s, e) => {
                Console.WriteLine("Client received {0} bytes", e.BytesTransferred);
                client.Receive();
            };

            client.Disconnected += (s, e) => {
                Console.WriteLine("Disconnected: " + e.SocketError);
                client.Connect();
            };

            client.Connected += (s, e) => {
                client.Receive();
            };

            client.Connect();
            Console.ReadLine();
        }
    }

    public class ExceptionEventArgs<TException> : EventArgs where TException : Exception {
        public TException Exception { get; private set; }

        public ExceptionEventArgs(TException exception) {
            Exception = exception;
        }
    }

    public class ExceptionEventArgs : ExceptionEventArgs<Exception> {
        public ExceptionEventArgs(Exception exception)
            : base(exception) {
        }

        public static implicit operator ExceptionEventArgs(Exception value) {
            return new ExceptionEventArgs(value);
        }

        public static implicit operator Exception(ExceptionEventArgs value) {
            return value.Exception;
        }
    }


    public static partial class Extensions {
        public static bool Connected(this Socket self) {
            return !(self.Poll(1000, SelectMode.SelectRead)) & (self.Available == 0);
        }
    }
}