using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hOTPcommon;

namespace hOTPwin {
	public class TOTPwin : TOTP {

		public Card Card { get; set; }

		public TOTPwin(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null, Period period = Period.ThirtySeconds, Digits digits = Digits.Six, string? issuer = null, string? account = null) : base(algorithm, secretKey, period, digits, issuer, account) {
			Card = new Card {
				Account = account,
				Issuer = issuer
			};
		}

		protected override void DisplayCode(object? state) {
			lock (codeLock) {
				if (currentCode == null) return;
				Card.Code = currentCode.Value;
				Card.TimeRemaining = currentCode.TimeRemaining;
				currentCode.TimeRemaining--;
			}
		}
	}
}
