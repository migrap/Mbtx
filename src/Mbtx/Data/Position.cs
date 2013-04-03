
using Newtonsoft.Json;
namespace Mbtx.Data {
    public class Position {
        public string Symbol { get; set; }
        public string Account { get; set; }
        public int AggregatePosition { get; set; }
        public double AveragePrice2 { get; set; }
        public int CloseableShares { get; set; }
        public double Commission { get; set; }
        public int IntradayPosition { get; set; }
        public double IntradayPrice { get; set; }
        public double MmrPct { get; set; }
        public double MmrUsed { get; set; }
        public int OvernightPosition { get; set; }
        public double OvernightPrice { get; set; }
        public int PendingBuyShares { get; set; }
        public int PendingSellShares { get; set; }
        public double RealizedPnl { get; set; }
        public double RealizedPnl2 { get; set; }
        public int StrategyShares { get; set; }

        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}