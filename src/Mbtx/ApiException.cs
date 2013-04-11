using System;
using System.Net;
using System.Net.Http;

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
