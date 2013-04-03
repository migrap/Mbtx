
using System.Text.RegularExpressions;
namespace Mbtx.Net.Http.Formatting {
    internal static partial class Extensions {
        public static int ToInt32(this Group self) {
            return int.Parse(self.Value);
        }
    }
}
