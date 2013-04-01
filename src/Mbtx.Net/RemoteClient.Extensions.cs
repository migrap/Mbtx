using Mbtx.Net.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mbtx.Net {
    public static partial class Extensions {
        public async static Task<string> GetAboutAsync(this RemoteClient client) {
            return await client.GetAsync<string>("about");
        }

        public async static Task<bool> GetConnectedAsync(this RemoteClient client) {
            return await client.GetAsync<bool>("connected");
        }

        public async static Task<string> GetVersionAsync(this RemoteClient client) {
            return await client.GetAsync<string>("version");
        }

        public async static Task<Process> GetProcessAsync(this RemoteClient client) {
            return await client.GetAsync<Process>("process");
        }

        public async static Task<string> RegisterAsync(this RemoteClient client, string id) {
            return await client.GetAsync<string>("register", new { id = id });
        }

        public async static Task<Accounts> GetAccountsAsync(this RemoteClient client) {
            return await client.GetAsync<Accounts>("accounts");
        }

        public async static Task<string> GetPositionsAsync(this RemoteClient client) {
            return await client.GetAsync<string>("positions");
        }

        public async static Task<Positions> GetPositionsAsync(this RemoteClient client, Account account) {
            return await client.GetAsync<Positions>("positions", account.AccountIdentifier);
        }

        public async static Task<Routes> GetRoutessAsync(this RemoteClient client) {
            return await client.GetAsync<Routes>("routes");
        }

        public async static Task<OrderHistorys> GetOrderHistoryAsync(this RemoteClient client) {
            return await client.GetAsync<OrderHistorys>("orderhistory");
        }

        public async static Task<OrderHistorys> GetOrderHistoryAsync(this RemoteClient client, Account account) {
            return await client.GetAsync<OrderHistorys>("orderhistory", account.AccountIdentifier);
        }

        public async static Task<Orders> GetOrdersAsync(this RemoteClient client) {
            return await client.GetAsync<Orders>("openorders");
        }

        public async static Task<Orders> GetOrdersAsync(this RemoteClient client, Account account) {
            return await client.GetAsync<Orders>("openorders", account.AccountIdentifier);
        }

        public async static Task<Alerts> GetAlertsAsync(this RemoteClient client) {
            return await client.GetAsync<Alerts>("alerts");
        }

        public async static Task<Strings> GetTypedValues(this RemoteClient client, string type) {
            return await client.GetAsync<Strings>("typedvalues", type);
        }
    }
}