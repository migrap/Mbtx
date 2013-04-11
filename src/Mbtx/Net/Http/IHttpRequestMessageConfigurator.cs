
namespace Mbtx.Net.Http {
    internal interface IHttpRequestMessageConfigurator {
        void Path(string value);
        void Values(object values);
    }
}
