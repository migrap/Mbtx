using System;

namespace Mbtx.Data {
    public class HistoryAdded {
        public string Account { get; set; }
        public BuySell BuySell { get; set; }
        public Capacity Capacity { get; set; }
        public int ContractsAhead { get; set; }
        public string Date { get; set; }
        public double DiscretionaryPrice { get; set; }
        public int DisplayQuantity { get; set; }
        public string Event { get; set; }
        public string ExecId { get; set; }
        public string Message { get; set; }
        public string OrderNumber { get; set; }
        public int OrderType { get; set; }
        public string OrigTraderId { get; set; }
        public string PrefMmid { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string RemoteId { get; set; }
        public string Route { get; set; }
        public int SharesFilled { get; set; }
        public double StopLimit { get; set; }
        public string Symbol { get; set; }
        public string Time { get; set; }
        public TimeInForce TimeInForce { get; set; }
        public string Token { get; set; }
        public DateTimeOffset UtcDateTime { get; set; }
        public int VolumeType { get; set; }
    }
}