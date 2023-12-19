using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hOTP {
	public class Code {
		public string Value { get; }
		public long TimeRemaining { get; }
		public Code(string value, long timeRemaining) {
			Value = value;
			TimeRemaining = timeRemaining;
		}

		public override string ToString() {
			return $"{Value} ({TimeRemaining:D2}s)";
		}
	}
}
