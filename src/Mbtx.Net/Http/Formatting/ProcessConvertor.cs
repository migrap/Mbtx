using Mbtx.Net.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mbtx.Net.Http.Formatting {
    public class ProcessConvertor : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Process);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            bool here = true;
            return new Process();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
