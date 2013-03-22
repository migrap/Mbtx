using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Net {
    internal static class Extensions {
        public static void Copy<T>(this T[] sourceArray, T[] destinationArray, int length) {
            Array.Copy(sourceArray, destinationArray, length);
        }

        public static void Copy<T>(this T[] sourceArray, T[] destinationArray, long length) {
            Array.Copy(sourceArray, destinationArray, length);
        }

        public static void Copy<T>(this T[] sourceArray, int sourceIndex, T[] destinationArray, int destinationIndex, int length) {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        public static void Copy<T>(this T[] sourceArray, long sourceIndex, T[] destinationArray, long destinationIndex, long length) {
            Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, length);
        }

        public static IEnumerable<IEnumerable<T>> Rows<T>(this T[][] source) {
            return source;
        }
        public static byte[] GetBytesTransfered(this SocketAsyncEventArgs source) {
            byte[] buffer = new byte[source.BytesTransferred];
            source.Buffer.Copy(source.Offset, buffer, 0, source.BytesTransferred);
            return buffer;
        }

        public static int GetBytesTransfered(this SocketAsyncEventArgs source, byte[] destination) {
            source.Buffer.Copy(source.Offset, destination, 0, source.BytesTransferred);
            return source.BytesTransferred;
        }        

        public static SocketAsyncOperation GetLastOperation(this SocketAsyncEventArgs source) {
            return GetLastOperation(source, (s) => s.AcceptSocket);
        }

        public static SocketAsyncOperation GetLastOperation(this SocketAsyncEventArgs source, Func<SocketAsyncEventArgs, Socket> socketSelector) {
            return ((source.BytesTransferred == 0) && (source.LastOperation.EqualsAny(SocketAsyncOperation.Receive, SocketAsyncOperation.ReceiveFrom, SocketAsyncOperation.ReceiveMessageFrom)) && (false == socketSelector(source).IsConnected())) ? SocketAsyncOperation.Disconnect : source.LastOperation;
        }

        public static bool EqualsAny<T>(this T obj, params T[] values) {
            return (Array.IndexOf(values, obj) != -1);
        }

        public static bool IsConnected(this Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        public static bool IsNullOrEmpty(this string value) {
            return string.IsNullOrEmpty(value);
        }

        public static string FormatWith(this string input, params object[] formatting) {
            return string.Format(input, formatting);
        }

        public static void Foreach<T>(this IEnumerable<T> self, Action<T> action) {
            var items = self.ToArray();
            for (int i = 0; i < items.Length; i++) {
                var item = items[i];
                action(item);
            }
        }

        internal static IPEndPoint Parse(this string value) {
            var parts = value.Split(':');
            if (parts.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress address;
            if (parts.Length > 2) {
                if (!IPAddress.TryParse(string.Join(":", parts, 0, parts.Length - 1), out address)) {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else {
                if (!IPAddress.TryParse(parts[0], out address)) {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(parts[parts.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port)) {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(address, port);
        }

        internal static void Send(this Socket client, string value) {
            Send(client, value, Encoding.ASCII);
        }

        internal static void Send(this Socket client, string value, Encoding encoding) {
            client.Send(encoding.GetBytes(value));
        }

        internal static void Receive(this Socket socket, Action<SocketAsyncEventArgs> receive, Action<SocketAsyncEventArgs> disconnect = null) {
            var arguments = new SocketAsyncEventArgs();
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

        public static string GetLogonDeniedReason(this string message) {
            return message.Substring(message.IndexOf("103=") + "103=".Length).Replace("\n", ": ");
        }
    }
}
