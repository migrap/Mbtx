using Mbtx.Net.Http;
using Mbtx.Net.Http.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Mbtx.Net {
    public class RemoteClient {
        private IEnumerable<MediaTypeFormatter> _formatters = new[] { new RemoteMediaTypeFormatter() };
        private HttpClient _client;
        private HttpMessageHandler _handler;


        public RemoteClient(string scheme = "http", string host = "127.0.0.1", int port = 8000, string path = "desktopservice") {
            _handler = new RemoteDelegatingHandler();

            var builder = new UriBuilder { Scheme = scheme, Host = host, Port = port, Path = path };

            _client = new HttpClient(_handler) {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = builder.Uri,
            };
        }

        internal Task<T> GetAsync<T>(HttpRequestMessage request) {
            request.RequestUri = _client.BaseAddress.Append(request.RequestUri.OriginalString);
            return _client.SendAsync(request).ContinueWith(x => x.Result.Content.ReadAsAsync<T>(_formatters)).Unwrap();
        }
    }
}