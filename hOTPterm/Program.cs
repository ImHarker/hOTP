using hOTPcommon;

namespace hOTPterm {
	public class Program {
		static void Main(string[] args) {
			Console.WriteLine("Press any key to close...");
			//TOTPterm totp = new TOTPterm(HashAlgorithm.SHA1, null, Period.ThirtySeconds, Digits.Six, "Imharker", "root@imharker.dev");
			//totp.GenerateQrCode();


			TOTPterm? totp = TOTPterm.DecodeQrCode("qrcode.png");
			if (totp == null) return;

			Console.ReadKey();

		}
	}
}