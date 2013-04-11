using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net.Http {
    internal interface IHttpRequestMessageConfigurator {
        void Path(string value);
        void Values(object values);
    }
}
