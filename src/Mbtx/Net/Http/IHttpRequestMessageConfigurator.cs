
using System.Net.Http.Formatting;
namespace Mbtx.Net.Http {
    internal interface IHttpRequestMessageConfigurator {
        void Path(string value);
        void Values(object values);
        void Content(object value);
        void Formatter(MediaTypeFormatter value);
    }
}
