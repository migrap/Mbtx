using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Data {
    public class OrderInfo {
        public BuySell BuySell { get; set; }
        public int Volume { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double StopPrice { get; set; }
        public TimeInForce Tif { get; set; }
        public Capacity Capacity { get; set; }
        public OrderType OrderType { get; set; }
        public Vol VolType { get; set; }
        public int DisplayQuantity { get; set; }
        public string Account { get; set; }
        public string Market { get; set; }
        public double PriceOther { get; set; }
        public double PriceOther2 { get; set; }
        public DateTimeOffset UtcTimeExpire { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public double StrikePrice { get; set; }
        public string ConditionalSymbol { get; set; }
        public ConditionalOrder ConditionalOrderType { get; set; }
        public double ConditionalPriceDesired { get; set; }
        public Group GroupType { get; set; }
        public string GroupId { get; set; }
        public bool ManuallyPlaced { get; set; }
        public string OrderToken { get; set; }
        public string Message { get; set; }
    }
}
