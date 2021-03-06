﻿using System;

namespace Mbtx.Data {
    public class Alert {
        public Category Category { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
        public bool Processed { get; set; }
        public Severity Severity { get; set; }
        public string Time { get; set; }
        public string Url { get; set; }
        public DateTimeOffset UtcDateTime { get; set; }
    }
}