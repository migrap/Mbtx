using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Mbtx.Net.Http.Formatting {
    internal class RemoteMediaTypeFormatter : JsonMediaTypeFormatter {
        public RemoteMediaTypeFormatter() {
            //SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            SerializerSettings.Converters.Add(new ProcessConvertor());
            SerializerSettings.Converters.Add(new ColorConverter());
            SerializerSettings.Converters.Add(new EncodingConverter());
            SerializerSettings.Converters.Add(new VersionConverter());
            SerializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
        }

        public override bool CanReadType(Type type) {
            return true;
        }

        public override bool CanWriteType(Type type) {
            return true;
        }
    }
}