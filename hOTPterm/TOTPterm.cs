using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hOTPcommon;

namespace hOTPterm {
	public class TOTPterm : TOTP {
		public TOTPterm(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null,
			Period period = Period.Thirty, Digits digits = Digits.Six, string? issuer = null,
			string? account = null) : base(algorithm, secretKey, period, digits, issuer, account) {
		}

		protected override void DisplayCode(object? state) {
			lock (codeLock) {
				if (currentCode == null) return;
				Console.WriteLine(currentCode);
				currentCode.TimeRemaining--;
			}
		}

		public void GenerateQrCode() {
			base.GenerateQrCode();
		}

		public static TOTPterm? DecodeQrCode(string path) {
			TOTP? totp = TOTP.DecodeQrCode(path);
			if (totp == null) return null;
			return new TOTPterm(totp.Algorithm, totp.SecretKey, totp.Period, totp.Digits, totp.Issuer, totp.Account);
		}

	}
}
