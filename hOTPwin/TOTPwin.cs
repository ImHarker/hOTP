using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hOTPcommon;

namespace hOTPwin {
	public class TOTPwin : TOTP {

		public Card? Card { get; set; }

		public TOTPwin(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null, Period period = Period.Thirty, Digits digits = Digits.Six, string? issuer = null, string? account = null) : base(algorithm, secretKey, period, digits, issuer, account) {
			Card = new Card {
				Account = account,
				Issuer = issuer
			};
		}

		protected override void DisplayCode(object? state) {
			lock (codeLock) {
				if (currentCode == null || Card == null) return;
				Card.Code = currentCode.Value;
				Card.TimeRemaining = currentCode.TimeRemaining;
				currentCode.TimeRemaining--;
			}
		}

		public Bitmap GenerateQrCode() {
			return base.GenerateQrCode();
		}

		public static TOTPwin? DecodeQrCode(string path) {
			TOTP? totp = TOTP.DecodeQrCode(path);
			if (totp == null) return null;
			return new TOTPwin(totp.Algorithm, totp.SecretKey, totp.Period, totp.Digits, totp.Issuer, totp.Account);
		}

		public static TOTPwin? DecodeURI(string uri) {
			TOTP? totp = TOTP.DecodeURI(uri);
			if (totp == null) return null;
			return new TOTPwin(totp.Algorithm, totp.SecretKey, totp.Period, totp.Digits, totp.Issuer, totp.Account);
		}

	}
}
