
namespace Mbtx.Data {
    public class OrderHistory {
        private string _message;

        public string Account { get;  set; }
        public BuySell BuySell { get;  set; }
        public Capacity Capacity { get; set; }
        public int ContractsAhead { get;  set; }
        public string Date { get;  set; }
        public double DiscretionaryPrice { get;  set; }
        public int DisplayQuantity { get;  set; }
        public string Event { get;  set; }
        public string ExecId { get;  set; }
        public string Message { get { return _message; } set { _message = value.Trim(); } }
        public int NumLegs { get;  set; }
        public string OrderNumber { get;  set; }
        public OrderType OrderType { get;  set; }
        public string OrigTraderId { get;  set; }
        public string PrefMmid { get;  set; }
        public double Price { get;  set; }
        public int Quantity { get;  set; }
        public string RemoteId { get;  set; }
        public string Route { get;  set; }
        public int SharesFilled { get;  set; }
        public double StopLimit { get;  set; }
        public string Symbol { get;  set; }
        public string Time { get;  set; }
        public int TimeInForce { get;  set; }
        public string Token { get;  set; }
        public string TraderId { get;  set; }
        //public DateTime DateTime { get;  set; }
        public Volume VolumeType { get; set; }
    }
}

/*
"Account": "E1724077",
"BuySell": 10000,
"Capacity": 0,
"ContractsAhead": 0,
"Date": "4\/1\/2013",
"DiscretionaryPrice": 0,
"DisplayQuantity": 0,
"Event": "Enter",
"ExecId": 1,
"Message": "Buy 1000 EUR\/USD @ 1.28492 Limit on MBTX [expiring: 6\/30\/2013]",
"NumLegs": 0,
"OrderNumber": "2bme70a:09or",
"OrderType": 10030,
"OrigTraderId": "CASNEV",
"PrefMmid": "",
"Price": 1.28492,
"Quantity": 1000,
"RemoteId": "",
"Route": "MBTX",
"SharesFilled": 0,
"StopLimit": 0,
"StrategyLegs": [],
"Symbol": "EUR\/USD",
"Time": "12:32:12",
"TimeInForce": 10008,
"Token": "22d32d28-casnev-1",
"TraderId": "CASNEV",
"UtcDateTime": "\/Date(1364859132000-0700)\/",
"VolumeType": 0
*/