﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net {
    public class LogonException : Exception {
        public LogonException(string message)
            : base(message) {
        }
    }
}