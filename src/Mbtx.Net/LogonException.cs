using System;

namespace Mbtx.Net {
    public class LogonException : Exception {
        public LogonException(string message)
            : base(message) {
        }
    }
}
