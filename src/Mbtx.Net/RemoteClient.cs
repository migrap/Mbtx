using Mbtx.Net.Http;
using Mbtx.Net.Http.Formatting;
using Mbtx.Net.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Mbtx.Net {
    public class RemoteClient {
        private BlockingCollection<StreamContent> _streams = new BlockingCollection<StreamContent>();
        private IEnumerable<MediaTypeFormatter> _formatters = new[] { new RemoteMediaTypeFormatter() };
        private HttpClient _http;
        private HttpMessageHandler _handler;
        private SocketClient _socket;
        private Uri _uri;
        private Protomod _protomod;

        public RemoteClient(string scheme = "http", string host = "127.0.0.1", int port = 8000, string path = "desktopservice") {
            _handler = new RemoteDelegatingHandler();

            _streams.GetConsumingEnumerable()
                .ToObservable(Scheduler.Default)
                .Subscribe(OnStream);

            var builder = new UriBuilder { Scheme = scheme, Host = host, Port = port, Path = path };
            _uri = builder.Uri;

            _http = new HttpClient(_handler) {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = _uri,
            };
        }

        public Uri BaseUri {
            get { return _uri; }
        }

        public async Task ConnectAsync(Protomod value) {
            _protomod = value;
            _socket = _socket ?? new SocketClient(value.Encoding);
            await _socket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, value.Port))
                .ContinueWith(x => _socket.Receive(OnReceive, OnDisconnect));
        }

        private void OnReceive(SocketAsyncEventArgs e) {
            e.GetProtomodContent(_protomod.Encoding).Foreach(_streams.Add);
            
            if (_socket.Connected) {
                _socket.ReceiveAsync(e);
            }
        }

        private void OnDisconnect(SocketAsyncEventArgs e) {
        }

        private void OnStream(StreamContent value) {
            if (value.Headers.ContentType != null) {
                var formatter = _formatters.First(x => x.SupportedMediaTypes.Contains(value.Headers.ContentType));
                if (value.Headers.Any(x => x.Key.Equals("x-type") && x.Value.Any(y => y == "Submit"))) {
                    value.ReadAsAsync<OrderHistory>(_formatters)
                        .ContinueWith(x => x.Result);
                }
            }
        }

        internal Task<T> GetAsync<T>(HttpRequestMessage request) {
            request.RequestUri = _http.BaseAddress.Append(request.RequestUri.OriginalString);

            return _http.SendAsync(request)
                .ContinueWith(x => x.Result.Content.ReadAsAsync<T>(_formatters))
                .Unwrap();
        }
    }
}