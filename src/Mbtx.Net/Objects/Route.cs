
namespace Mbtx.Net.Objects {
    public class Route {
        public bool AllowConditional { get; internal set; }
        public bool AllowCoveredCalls { get; internal set; }
        public bool AllowDay { get; internal set; }
        public bool AllowDayPlus { get; internal set; }
        public bool AllowDiscReserve { get; internal set; }
        public bool AllowDiscretionary { get; internal set; }
        public bool AllowForexPNP { get; internal set; }
        public bool AllowGTC { get; internal set; }
        public bool AllowGTX { get; internal set; }
        public bool AllowIOC { get; internal set; }
        public bool AllowLOC { get; internal set; }
        public bool AllowLOO { get; internal set; }
        public bool AllowMOC { get; internal set; }
        public bool AllowMOO { get; internal set; }
        public bool AllowMarket { get; internal set; }
        public bool AllowOptionsFutures { get; internal set; }
        public bool AllowPegged { get; internal set; }
        public bool AllowPref { get; internal set; }
        public bool AllowReplace { get; internal set; }
        public bool AllowReserve { get; internal set; }
        public bool AllowStop { get; internal set; }
        public bool AllowStopLimit { get; internal set; }
        public bool AllowTimeTriggered { get; internal set; }
        public bool AllowTrailingStop { get; internal set; }
        public bool AllowTto { get; internal set; }
        public bool AllowVwap { get; internal set; }
        public string Name { get; internal set; }
    }
}