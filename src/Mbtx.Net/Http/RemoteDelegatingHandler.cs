using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mbtx.Net.Http {
    internal class RemoteDelegatingHandler : DelegatingHandler {
        public RemoteDelegatingHandler() {
            InnerHandler = new HttpClientHandler {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None,                
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken);
        }
    }
}