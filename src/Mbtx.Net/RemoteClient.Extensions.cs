using Mbtx.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net
{
    public static partial class Extensions {
        public async static Task<string> GetVersionAsync(this RemoteClient client) {
            return await client.GetAsync<string>("version");
        }

        public async static Task<bool> GetConnectedAsync(this RemoteClient client) {
            return await client.GetAsync<bool>("connected");
        }

        public async static Task<string> GetProcessAsync(this RemoteClient client) {
            return await client.GetAsync<string>("process");
        }
    }
}
