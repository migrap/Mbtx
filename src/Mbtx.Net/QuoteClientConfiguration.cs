﻿using System.ComponentModel;

namespace Mbtx.Net {
    public class QuoteClientConfiguration {
        [DefaultValue(5020)]
        public int Port { get; set; }

        [DefaultValue(8096)]
        public int ReceiveBufferSize { get; set; }
    }
}