using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbtx.Data {
    public class Handle {
        private string _value;

        private Handle(string value) {
            _value = value;
        }

        public override string ToString() {
            return _value;
        }

        public static implicit operator Handle(string value) {
            return new Handle(value);
        }

        public static implicit operator string(Handle value) {
            return value.ToString();
        }
    }
}
