using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx {
    public class ApiException : Exception {
        public HttpStatusCode Status { get; protected set; }
        public HttpResponseMessage ResponseMessage { get; protected set; }

        public ApiException(HttpResponseMessage responseMessage, string message, Exception innerException = null)
            : base(message, innerException) {
            ResponseMessage = responseMessage;
            Status = responseMessage.StatusCode;
        }
    }
}
