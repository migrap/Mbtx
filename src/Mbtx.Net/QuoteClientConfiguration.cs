using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net {
    public class QuoteClientConfiguration {
        [DefaultValue(5020)]
        public int Port { get; set; }

        [DefaultValue(8096)]
        public int ReceiveBufferSize { get; set; }
    }
}