using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mbtx.Net.Http.Formatting {
    internal class RemoteMediaTypeFormatter : JsonMediaTypeFormatter {
        public RemoteMediaTypeFormatter() {
            //SerializerSettings.Converters.Add(new ProcessConvertor());
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