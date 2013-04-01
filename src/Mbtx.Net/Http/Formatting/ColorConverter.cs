using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Mbtx.Net.Http.Formatting {
    public class ColorConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Color);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return (Color)existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}