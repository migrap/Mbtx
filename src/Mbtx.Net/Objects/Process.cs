using Mbtx.Net.Http.Formatting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net.Objects {
    [JsonConverter(typeof(ProcessConvertor))]
    public class Process {
        public string ProcessId { get; set; }
        public string ThreadId { get; set; }
    }
}
