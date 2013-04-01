using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net.Objects {
    public enum AccountState {
        Unloaded = 0,
        Loading = 1,
        Reloading = 2,
        Loaded = 3,
        Unavailable = 4,
    }
}