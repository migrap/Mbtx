using Newtonsoft.Json;
using System;
using System.Text;

namespace Mbtx.Net.Http.Formatting {
    public class EncodingConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Encoding);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return Encoding.GetEncoding(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}