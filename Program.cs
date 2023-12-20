using System.Text;

namespace hOTP {
	public class Program {
		static void Main(string[] args) {

			TOTP totp = new TOTP(HashAlgorithm.SHA1, null, 30, 6, "Imharker", "root@imharker.dev");
			totp.GenerateQrCode();

			//TOTP? totp = TOTP.DecodeQrCode("qrcode.png");
			//if (totp == null) return;


			for (int i = 0; i < 30; i++) {
				Console.WriteLine($"{totp.GetCode()}");
				Thread.Sleep(1000);
			}
		}
	}
}