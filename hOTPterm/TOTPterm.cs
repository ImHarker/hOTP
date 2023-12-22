using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hOTPcommon;

namespace hOTPterm {
	public class TOTPterm : TOTP {
		public TOTPterm(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null, Period period = Period.ThirtySeconds, Digits digits = Digits.Six, string? issuer = null, string? account = null) : base(algorithm, secretKey, period, digits, issuer, account) {
		}

		protected override void DisplayCode(object? state) {
			lock (codeLock) {
				if (currentCode == null) return;
				Console.WriteLine(currentCode);
				currentCode.TimeRemaining--;
			}
		}
	}
}
