using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx {
    public static partial class Extensions {
        internal static void Copy<T>(this T[] sourceArray, T[] destinationArray, int length) {
            Array.Copy(sourceArray, destinationArray, length);
        }

        internal static void Copy<T>(this T[] sourceArray, T[] destinationArray, long length) {
            Array.Copy(sourceArray, destinationArray, length);
        }

        internal static void Copy<T>(this T[] sourceArray, int sourceIndex, T[] destinationArray, int destinationIndex, int length) {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        internal static void Copy<T>(this T[] sourceArray, long sourceIndex, T[] destinationArray, long destinationIndex, long length) {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        internal static IEnumerable<IEnumerable<T>> Rows<T>(this T[][] source) {
            return source;
        }

        internal static byte[] GetBytesTransfered(this SocketAsyncEventArgs source) {
            byte[] buffer = new byte[source.BytesTransferred];
            source.Buffer.Copy(source.Offset, buffer, 0, source.BytesTransferred);
            return buffer;
        }

        internal static int GetBytesTransfered(this SocketAsyncEventArgs source, byte[] destination) {
            source.Buffer.Copy(source.Offset, destination, 0, source.BytesTransferred);
            return source.BytesTransferred;
        }

        internal static SocketAsyncOperation GetLastOperation(this SocketAsyncEventArgs source) {
            return GetLastOperation(source, (s) => s.AcceptSocket);
        }

        internal static SocketAsyncOperation GetLastOperation(this SocketAsyncEventArgs source, Func<SocketAsyncEventArgs, Socket> socketSelector) {
            return ((source.BytesTransferred == 0) && (source.LastOperation.EqualsAny(SocketAsyncOperation.Receive, SocketAsyncOperation.ReceiveFrom, SocketAsyncOperation.ReceiveMessageFrom)) && (false == socketSelector(source).IsConnected())) ? SocketAsyncOperation.Disconnect : source.LastOperation;
        }

        internal static bool EqualsAny<T>(this T obj, params T[] values) {
            return (Array.IndexOf(values, obj) != -1);
        }

        internal static bool IsConnected(this Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        internal static bool IsNullOrEmpty(this string value) {
            return string.IsNullOrEmpty(value);
        }

        internal static string FormatWith(this string input, params object[] formatting) {
            return string.Format(input, formatting);
        }

        internal static void Foreach<T>(this IEnumerable<T> self, Action<T> action) {
            var items = self.ToArray();
            for (int i = 0; i < items.Length; i++) {
                var item = items[i];
                action(item);
            }
        }

        internal static void Send(this Socket client, string value) {
            Send(client, value, Encoding.ASCII);
        }

        internal static void Send(this Socket client, string value, Encoding encoding) {
            client.Send(encoding.GetBytes(value));
        }

        internal static void Receive(this Socket socket, Action<SocketAsyncEventArgs> receive, Action<SocketAsyncEventArgs> disconnect = null) {
            var arguments = new SocketAsyncEventArgs();
            arguments.UserToken = socket;
            arguments.SetBuffer(new byte[8096], 0, 8096);

            var observable = from args in Observable.FromEventPattern<SocketAsyncEventArgs>(
                             ev => arguments.Completed += ev,
                             ev => arguments.Completed -= ev)
                             select args.EventArgs;

            observable.Where(args => args.LastOperation == SocketAsyncOperation.Receive)
                .ObserveOn(Scheduler.Default)
                .Subscribe(receive);

            if (null != disconnect) {
                observable.Where(args => args.LastOperation == SocketAsyncOperation.Disconnect)
                    .ObserveOn(Scheduler.Default)
                    .Subscribe(disconnect);
            }

            if (socket.Connected) {
                socket.ReceiveAsync(arguments);
            }
        }

        internal static string GetLogonDeniedReason(this string message) {
            return message.Substring(message.IndexOf("103=") + "103=".Length).Replace("\n", ": ");
        }

        internal static IEnumerable<PropertyDescriptor> Where(this PropertyDescriptorCollection collection, Func<PropertyDescriptor, bool> predicate) {
            foreach (PropertyDescriptor item in collection) {
                if (predicate(item)) {
                    yield return item;
                }
            }
        }

        internal static string Join(this IEnumerable<object> source, string seperator) {
            return string.Join(seperator, source);
        }

        internal static Uri Append(this Uri self, params object[] segments) {
            return new Uri(segments.Aggregate(self.AbsoluteUri, (current, segment) => string.Format("{0}/{1}", current.TrimEnd('/'), segment)));
        }

        internal static IEnumerable<StreamContent> GetStreamContent(this SocketAsyncEventArgs self) {
            return GetStreamContent(self, Encoding.UTF8);
        }

        internal static IEnumerable<StreamContent> GetStreamContent(this SocketAsyncEventArgs self, Encoding encoding) {
            var buffer = self.GetBytesTransfered();
            int i = 0, j = 0, k = 0, l = 0;

            for (i = 0; i < buffer.Length; i++) {
                for (j = i, k = 0; j < buffer.Length && k != 0x0d0a0d0a; j++) {
                    var ch = buffer[j];
                    k = (k << 8) | ch;
                }

                var headers = encoding.GetString(buffer, i, j - i);

                var stream = new MemoryStream();
                var content = new StreamContent(stream);

                headers.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                   .Foreach(x => {
                       var parts = x.Split(":".ToCharArray()).Select(s => s.Trim()).ToArray();
                       var name = parts[0].ToLower();
                       var value = parts[1];

                       content.Headers.Add(name, value);
                   });

                i = (int)content.Headers.ContentLength.Value;
                stream.Write(buffer, j, i);
                stream.Seek(0, SeekOrigin.Begin);

                yield return content;

                i = j + i;
            }
            yield break;
        }

        internal static void Raise<TEventArgs>(this EventHandler<TEventArgs> self, object sender, TEventArgs e) where TEventArgs : EventArgs {
            var handle = self;
            if (null != handle) {
                handle(sender, e);
            }
        }

        internal static string ToQueryString(this object values) {
            return (values == null) ? string.Empty : TypeDescriptor.GetProperties(values)
                .Where(x => x.GetValue(values) != null)
                .Select(x => "{0}={1}".FormatWith(x.Name.UrlEncode(), x.GetValue(values).UrlEncode()))
                .Join("&");
        }

        /// <summary>
        /// Uses Uri.EscapeDataString() based on recommendations on MSDN
        /// http://blogs.msdn.com/b/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
        /// </summary>
        internal static string UrlEncode(this string self) {
            return Uri.EscapeDataString(self);
        }

        internal static string UrlEncode(this object self) {
            return UrlEncode(self.ToString());
        }

        public static async Task EnsureSuccessStatusCode(this HttpResponseMessage message, bool @throw = true) {
            if (@throw && !message.IsSuccessStatusCode) {
                throw new ApiException(message, "The API query failed with status code {0}: {1}".FormatWith(message.StatusCode, message.ReasonPhrase));
            }
        }

        internal static Task<T> ReadAsAsync<T>(this HttpContent content, MediaTypeFormatter formatter) {
            return content.ReadAsAsync<T>(new[] { formatter });
        }
    }
}