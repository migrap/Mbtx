using Mbtx.Net.Objects;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Mbtx.Net.Http.Formatting {
    public class ProcessConvertor : JsonConverter {
        private Regex _regex = new Regex(@"Process ID: (?<process>[\d]+), Thread ID: (?<thread>[\d]+)", RegexOptions.Compiled);
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Process);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var value = reader.Value.ToString();
            var match = _regex.Match(value);

            var process = match.Groups["process"].AsInt32();
            var thread = match.Groups["thread"].AsInt32();

            return new Process {
                ProcessId = process,
                ThreadId = thread,
            };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
