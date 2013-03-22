using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net {
    public sealed class Quote {
        public string Symbol { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public double BidPrice { get; set; }
        public long BidSize { get; set; }
        public double AskPrice { get; set; }
        public long AskSize { get; set; }
        public double LastPrice { get; set; }
        public long LastSize { get; set; }

        public override string ToString() {
            return "{0} {1} | {2:0.000000} {3,10} | {4:0.000000} {5,10} | {6:0.000000} {7,10} {8}".FormatWith(Symbol, DateTime, BidPrice, BidSize, AskPrice, AskSize, LastPrice, LastSize, (BidPrice == LastPrice) ? "B" : (AskPrice == LastPrice) ? "A" : "M");
        }
    }    
}
