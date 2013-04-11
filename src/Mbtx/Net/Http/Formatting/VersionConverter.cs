using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mbtx.Net.Http.Formatting {
    public class VersionConverter : JsonConverter {
        private Regex _regex = new Regex(@"[0-9]+\.[0-9]+(?:\.[0-9]+)?", RegexOptions.Compiled);
        public override bool CanConvert(Type objectType) {
            return typeof(Version).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var value = reader.Value.ToString();
            return Version.Parse(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
