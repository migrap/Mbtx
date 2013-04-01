
namespace Mbtx.Net.Objects {
    public class Account {
        //public string AccountColor { get; set; }
        public string AccountIdentifier { get; set; }
        public int AccountType { get; set; }
        public string Bank { get; set; }
        public string BaseCurrency { get; set; }
        public string Branch { get; set; }
        public int CancelsToday { get; set; }
        public double Credit { get; set; }
        public double CurrentEquity { get; set; }
        public double CurrentExcess { get; set; }
        public double CurrentMinimumMarginRequirement { get; set; }
        public string CustomAccountName { get; set; }
        public string Customer { get; set; }
        public double DailyRealizedProfitLoss { get; set; }
        public double MarginMultiplier { get; set; }
        public double MorningCash { get; set; }
        public double MorningEquity { get; set; }
        public double MorningExcess { get; set; }
        public double OvernightExcess { get; set; }
        public string RoutingId { get; set; }
        public string SemiDelimited { get; set; }
        public double SharesToday { get; set; }
        public AccountState State { get; set; }
        public int TradesToday { get; set; }
    }
}