using System.Text;

namespace hOTP {
	public class Program {
		static void Main(string[] args) {

			//TOTP totp = new TOTP(HashAlgorithm.SHA1, null, Period.ThirtySeconds, Digits.Six, "Imharker", "root@imharker.dev");
			//Console.WriteLine(totp.URI); 
			//totp.GenerateQrCode();


			TOTP? totp = TOTP.DecodeQrCode("qrcode.png");
			if (totp == null) return;

			Thread.Sleep(1000 * 60);

		}
	}
}