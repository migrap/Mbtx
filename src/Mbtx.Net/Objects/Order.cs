using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net.Objects {
    public class Order {
        public bool Acknowledged { get; set; }
        public BuySell BuySell { get; set; }
        public Capacity Capacity { get; set; }
        public int ContractsAhead { get; set; }
        public string CurrentEvent { get; set; }
        public string Date { get; set; }
        public double DiscretionaryPrice { get; set; }
        public int DisplayQuantity { get; set; }
        public int NumLegs { get; set; }
        public string OrderNumber { get; set; }
        public OrderType OrderType { get; set; }
        public string PrefMmid { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public bool Replaceable { get; set; }
        public string Route { get; set; }
        public int SharesFilled { get; set; }
        public double StopLimit { get; set; }        
        public string Symbol { get; set; }
        public string Time { get; set; }
        public TimeInForce TimeInForce { get; set; }
        public string Token { get; set; }
        public string TraderId { get; set; }
        public double TrailingOffset { get; set; }
        public DateTimeOffset UtcDateTime { get; set; }
        public VolumeType VolumeType { get; set; }
    }
}

//"Acknowledged": true,
//"BuySell": 10000,
//"Capacity": 0,
//"ContractsAhead": 0,
//"CurrentEvent": "Live",
//"Date": "4\/1\/2013",
//"DiscretionaryPrice": 0,
//"DisplayQuantity": 0,
//"NumLegs": 0,
//"OrderNumber": "2bme70a:09u2",
//"OrderType": 10030,
//"PrefMmid": "",
//"Price": 1,
//"Quantity": 1000,
//"Replaceable": true,
//"Route": "MBTX",
//"SharesFilled": 0,
//"StopLimit": 0,
//"StrategyLegs": [],
//"Symbol": "EUR\/USD",
//"Time": "13:26:08",
//"TimeInForce": 10008,
//"Token": "22d32d28-casnev-4",
//"TraderId": "CASNEV",
//"TrailingOffset": 0,
//"UtcDateTime": "\/Date(1364862368000-0700)\/",
//"VolumeType": 0