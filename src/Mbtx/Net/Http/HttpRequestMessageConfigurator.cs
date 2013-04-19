using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Mbtx.Net.Http {
    internal class HttpRequestMessageConfigurator : IHttpRequestMessageConfigurator {
        private HttpMethod _method;
        private string _path;
        private object _values;
        private Uri _baseaddress;
        private object _content;
        private MediaTypeFormatter _formatter;

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
            Contract.Requires((_method == HttpMethod.Post) && (_content != null && _formatter != null));

            var builder = new UriBuilder(_baseaddress) {
                Query = _values.ToQueryString(),
            };

            builder.Path = "{0}/{1}".FormatWith(builder.Path, _path);

            var message = new HttpRequestMessage {
                Method = _method,
                RequestUri = builder.Uri,               
            };

            if (_content != null) {
                message.Content = new ObjectContent(_content.GetType(), _content, _formatter);
            }

            return message;
        }

        public void Content(object value) {
            _content = value;
        }

        public void Formatter(MediaTypeFormatter value) {
            _formatter = value;
        }
    }
}
