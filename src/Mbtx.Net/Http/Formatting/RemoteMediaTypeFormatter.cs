using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Mbtx.Net.Http.Formatting {
    internal class RemoteMediaTypeFormatter : JsonMediaTypeFormatter {
        public RemoteMediaTypeFormatter() {
            SerializerSettings.Converters.Add(new ProcessConvertor());
            SerializerSettings.Converters.Add(new ColorConverter());
        }

        public override bool CanReadType(Type type) {
            return true;
        }

        public override bool CanWriteType(Type type) {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger) {
            return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
        }
    }
}