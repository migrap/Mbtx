using Mbtx.Data;
using Mbtx.Net.Http;
using Mbtx.Net.Http.Formatting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;


namespace Mbtx.Net {
    public class RemoteClient {
        private BlockingCollection<StreamContent> _streams = new BlockingCollection<StreamContent>();
        private RemoteMediaTypeFormatter _formatter = new RemoteMediaTypeFormatter();
        private IEnumerable<RemoteMediaTypeFormatter> _formatters = new[] { new RemoteMediaTypeFormatter() };
        private HttpClient _http;
        private HttpMessageHandler _handler;
        private SocketClient _socket;
        private Protomod _protomod;
        
        private Subject<Position> _position = new Subject<Position>();
        private Subject<Transaction> _transaction = new Subject<Transaction>();

        public RemoteClient(string scheme = "http", string host = "127.0.0.1", int port = 8000, string path = "desktopservice") {
            _streams.GetConsumingEnumerable()
                .ToObservable(Scheduler.Default)
                .Subscribe(OnStreamContent);

            var builder = new UriBuilder { Scheme = scheme, Host = host, Port = port, Path = path };

            _http = new HttpClient(new RemoteDelegatingHandler()) {
                Timeout = TimeSpan.FromMinutes(5),
                BaseAddress = builder.Uri,
            };
        }

        public IObservable<Position> Position {
            get { return _position; }
        }

        public IObservable<Transaction> Transaction {
            get { return _transaction; }
        }

        public async Task ConnectAsync(Protomod value) {
            _protomod = value;
            _socket = _socket ?? new SocketClient(value.Encoding);
            await _socket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, value.Port))
                .ContinueWith(x => _socket.Receive(OnReceive, OnDisconnect));
        }

        private void OnReceive(SocketAsyncEventArgs e) {
            e.GetStreamContent(_protomod.Encoding).Foreach(_streams.Add);

            if (_socket.Connected) {
                _socket.ReceiveAsync(e);
            }
        }

        private void OnDisconnect(SocketAsyncEventArgs e) {
        }

        private void OnStreamContent(StreamContent value) {
            if (value.Headers.ContentType != null) {
                var header = value.Headers.FirstOrDefault(x => x.Key.Equals("x-type"));

                if (header.Equals(default(KeyValuePair<string, IEnumerable<string>>))) {
                }
                else if (header.Value.Contains("HistoryAdded")) {
                    value.ReadAsAsync<HistoryAdded>().ContinueWith(x => x.Result);
                }
                else if (header.Value.Contains("PositionAdded")) {
                    value.ReadAsAsync<Position>().ContinueWith(x => _position.OnNext(x.Result));
                }
                else if (header.Value.Contains("PositionUpdated")) {
                    value.ReadAsAsync<Position>().ContinueWith(x => _position.OnNext(x.Result));
                }
                else if (header.Value.Contains("Submit")) {
                    value.ReadAsAsync<Transaction>().ContinueWith(x => {
                        var result = x.Result;
                        result.Type = "Submit";
                        _transaction.OnNext(result);
                    });
                }
                else if (header.Value.Contains("Acknowledge")) {
                    value.ReadAsAsync<Transaction>().ContinueWith(x => {
                        var result = x.Result;
                        result.Type = "Acknowledge";
                        _transaction.OnNext(result);
                    });
                }
                else if (header.Value.Contains("CancelPlaced")) {
                    value.ReadAsAsync<Transaction>().ContinueWith(x => {
                        var result = x.Result;
                        result.Type = "CancelPlaced";
                        _transaction.OnNext(result);
                    });
                }
                else {
                    var result = value.ReadAsStringAsync().Result;
                    Console.WriteLine("{0} -> {1}", header.Value.First(), result);
                }


                //else if (header.Value.Contains("HealthUpdate")) {
                //}
                //else if (header.Value.Contains("AlertAdded")) {
                //}
                //else if (header.Value.Contains("AlertsRefresh")) {
                //}
                //else if (header.Value.Contains("CriticalShutdown")) {
                //}
                //else if (header.Value.Contains("LogonDeny")) {
                //}
                //else if (header.Value.Contains("LogonSucceed")) {
                //}
                //else if (header.Value.Contains("AccountLoaded")) {
                //}
                //else if (header.Value.Contains("AccountLoading")) {
                //}
                //else if (header.Value.Contains("AccountUnavailable")) {
                //}
                //else if (header.Value.Contains("AccountsListBuilt")) {
                //}
                //else if (header.Value.Contains("Acknowledge")) {
                //}
                //else if (header.Value.Contains("BalanceUpdate")) {
                //}
                //else if (header.Value.Contains("CancelPlaced")) {
                //}
                //else if (header.Value.Contains("CancelRejected")) {
                //}
                //else if (header.Value.Contains("ClientLogonSucceed")) {
                //}
                //else if (header.Value.Contains("Close")) {
                //}
                //else if (header.Value.Contains("Connected")) {
                //}
                //else if (header.Value.Contains("DefaultAccountChanged")) {
                //}
                //else if (header.Value.Contains("Execute")) {
                //}                
                //else if (header.Value.Contains("JournalSubmit")) {
                //}                
                //else if (header.Value.Contains("PositionStrategyGroupAdded")) {
                //}
                //else if (header.Value.Contains("PositionStrategyGroupRemoved")) {
                //}
                //else if (header.Value.Contains("PositionStrategyGroupUpdated")) {                }

                //else if (header.Value.Contains("Remove")) {
                //}
                //else if (header.Value.Contains("ReplacePlaced")) {
                //}
                //else if (header.Value.Contains("ReplaceRejected")) {
                //}
                //else if (header.Value.Contains("Submit")) {
                //}
                //else if (header.Value.Contains("BasketAdded")) {
                //}
                //else if (header.Value.Contains("BasketRemoved")) {
                //}
                //else if (header.Value.Contains("BasketRenamed")) {
                //}
                //else if (header.Value.Contains("ChartAdded")) {
                //}
                //else if (header.Value.Contains("ChartRemoved")) {
                //}
                //else if (header.Value.Contains("ChartRenamed")) {
                //}
                //else if (header.Value.Contains("FXBoardAdded")) {
                //}
                //else if (header.Value.Contains("FXBoardRemoved")) {
                //}
                //else if (header.Value.Contains("FXBoardRenamed")) {
                //}
                //else if (header.Value.Contains("PrefsChanged")) {
                //}
                //else if (header.Value.Contains("WatchlistAdded")) {
                //}
                //else if (header.Value.Contains("WatchlistRemoved")) {
                //}
                //else if (header.Value.Contains("WatchlistRenamed")) {
                //}
            }
        }        

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, Action<IHttpRequestMessageConfigurator> configure) {
            var configurator = new HttpRequestMessageConfigurator();

            configurator.Method(method);
            configurator.BaseAddress(_http.BaseAddress);
            configure(configurator);

            var request = configurator.Build();

            var response = await _http.SendAsync(request).ConfigureAwait(false);
            await response.EnsureSuccessStatusCode(true).ConfigureAwait(false);
            return response;
        }

        private async Task<HttpResponseMessage> GetAsync(string path, object values = null) {
            return await SendAsync(HttpMethod.Get, x => {
                x.Path(path);
                x.Values(values);
            });
        }

        private async Task<T> GetAsync<T>(string path, object values = null) {
            return await GetAsync(path, values)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .Content.ReadAsAsync<T>(_formatter)
                .ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PostAsync(string path) {
            return await SendAsync(HttpMethod.Post, x => {
                x.Path(path);
            });
        }

        private async Task<T> PostAsync<T>(string path) where T : class {
            return await PostAsync(path)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult()
                .Content.ReadAsAsync<T>(_formatter)
                .ConfigureAwait(false);
        }

        public async Task<string> GetAboutAsync() {
            return await GetAsync<string>("about");
        }

        public async Task<bool> GetConnectedAsync() {
            return await GetAsync<bool>("connected");
        }

        public async Task<Version> GetVersionAsync() {
            return await GetAsync<Version>("version");
        }

        public async Task<Process> GetProcessAsync() {
            return await GetAsync<Process>("process");
        }        

        public async Task<Accounts> GetAccountsAsync() {
            return await GetAsync<Accounts>("accounts");
        }

        public async Task<Positions> GetPositionsAsync() {
            return await GetAsync<Positions>("positions");
        }

        public async Task<Positions> GetPositionsAsync(Account account) {
            return await GetAsync<Positions>("positions", account.AccountIdentifier);
        }

        public async Task<Routes> GetRoutessAsync() {
            return await GetAsync<Routes>("routes");
        }

        public async Task<OrderHistorys> GetOrderHistoryAsync() {
            return await GetAsync<OrderHistorys>("orderhistory");
        }

        public async Task<OrderHistorys> GetOrderHistoryAsync(Account account) {
            return await GetAsync<OrderHistorys>("orderhistory", account.AccountIdentifier);
        }

        public async Task<Orders> GetOrdersAsync() {
            return await GetAsync<Orders>("openorders");
        }

        public async Task<Orders> GetOrdersAsync(Account account) {
            return await GetAsync<Orders>("openorders", account.AccountIdentifier);
        }

        public async Task<Alerts> GetAlertsAsync() {
            return await GetAsync<Alerts>("alerts");
        }

        public async Task<Strings> GetTypedValuesAsync(string type) {
            return await GetAsync<Strings>("typedvalues", type);
        }

        public async Task<Protomod> GetEventsAsync(string events = "HealthUpdate,Alerts,CriticalShutdown,Logon,Accounts,Acknowledge,BalanceUpdate,Cancel,Close,Connected,Execute,HistoryAdded,JournalSubmit,Remove,Replace,Submit,Baskets,Charts,PrefsChanged", int port = 0) {
            return await GetAsync<Protomod>("{0}/{1}/{2}/{3}".FormatWith("events", "protomod", events, (port) != 0 ? port : _http.BaseAddress.Port + 1));
        }

        //public async Task<Handle> RegisterAsync(string id) {
        //    var handle = await GetAsync<Handle>("register", id);
        //    Interlocked.Exchange<Handle>(ref _handle, handle);
        //    return handle;
        //}

        //public async Task<HttpResponseMessage> SubmitOrderAsync(OrderInfo order) {
        //    return await PostAsync<OrderInfo>(order, "submitorder", _handle);
        //}

        //public async Task<HttpResponseMessage> ReplaceOrderAsync(OrderInfo orderInfo, Order order) {
        //    return await PostAsync<OrderInfo>(orderInfo, "replaceorder", _handle, order.OrderNumber);
        //}

        //public async Task<HttpResponseMessage> CancelOrderAsync(Order order) {
        //    return await PostAsync<string>(String.Empty, "cancelorder", _handle, order.OrderNumber);
        //}
    }
}