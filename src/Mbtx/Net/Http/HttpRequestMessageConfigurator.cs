using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net.Http {
    internal class HttpRequestMessageConfigurator : IHttpRequestMessageConfigurator {
        private HttpMethod _method;
        private string _path;
        private object _values;
        private Uri _baseaddress;

        internal void BaseAddress(Uri value) {
            _baseaddress = value;
        }

        internal void Method(HttpMethod method) {
            _method = method;
        }

        public void Path(string value) {
            _path = value;
        }

        public void Values(object values) {
            _values = values;
        }

        public HttpRequestMessage Build() {
            var builder = new UriBuilder(_baseaddress) {
                Query = _values.ToQueryString(),
            };
            builder.Path = "{0}/{1}".FormatWith(builder.Path, _path);

            var message = new HttpRequestMessage {
                Method = _method,
                RequestUri = builder.Uri,
            };

            return message;
        }
    }
}
