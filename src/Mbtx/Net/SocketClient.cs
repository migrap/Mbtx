using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net {
    internal class SocketClient {
        private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private SocketAsyncEventArgs _receive;        
        private Encoding _encdoing;

        public SocketClient()
            : this(Encoding.ASCII) {
        }

        public SocketClient(Encoding encoding) {
            _encdoing = encoding;
            _receive = new SocketAsyncEventArgs();
            _receive.SetBuffer(new byte[8096],0,8096);
        }

        public async Task ConnectAsync(IPEndPoint endPoint) {            
            await Task.Run(() => _socket.Connect(endPoint));      
        }

        public void Receive(Action<SocketAsyncEventArgs> receive, Action<SocketAsyncEventArgs> disconnect = null) {
            _socket.Receive(receive, disconnect);
        }

        public void Send(string value) {
            _socket.Send(_encdoing.GetBytes(value));
        }

        public bool Connected {
            get { return _socket.Connected; }
        }

        public bool ReceiveAsync(SocketAsyncEventArgs e) {
            return _socket.ReceiveAsync(e);
        }
    }
}
