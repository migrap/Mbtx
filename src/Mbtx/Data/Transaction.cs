using Newtonsoft.Json;
using System;

namespace Mbtx.Data {
    public class Transaction {
        public string Type { get; set; }
        public bool Acknowledged { get; set; }
        public BuySell BuySell { get; set; }
        public Capacity Capacity { get; set; }
        public int ContractsAhead { get; set; }
        public string CurrentEvent { get; set; }
        public string Date { get; set; }
        public double DiscretionaryPrice { get; set; }
        public int DisplayQuantity { get; set; }
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
        public int VolumeType { get; set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}