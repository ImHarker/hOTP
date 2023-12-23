using System.Security.Cryptography;
using ZXing;
using HashAlgorithm = hOTPcommon.HashAlgorithm;

namespace hOTPcommon {
	public class TOTP {
		public HashAlgorithm Algorithm { get; }
		public Period Period { get; }
		public Digits Digits { get; }
		public string? Account { get; }
		public string? Issuer { get; }
		public string? SecretKey { get; }
		public string URI { get; }

		private HMAC? HMAC { get; }
		private Timer UpdateCodeTimer;
		private Timer displayCodeTimer;
		protected Code? currentCode;
		protected readonly object codeLock = new object();


		private void UpdateCode(object? state) {
			lock (codeLock) {
				currentCode = GetCode();
			}
		}

		protected virtual void DisplayCode(object? state) {
			lock (codeLock) {
				if (currentCode == null) return;
				currentCode.TimeRemaining--;
			}
		}

		protected TOTP(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null, Period period = Period.ThirtySeconds, Digits digits = Digits.Six, string? issuer = null, string? account = null) {
			Algorithm = algorithm;
			Period = period;
			Digits = digits;
			Account = account;
			Issuer = issuer;
			SecretKey = secretKey;

			HMAC = algorithm switch {
				HashAlgorithm.SHA1 => new HMACSHA1(),
				HashAlgorithm.SHA256 => new HMACSHA256(),
				HashAlgorithm.SHA512 => new HMACSHA512(),
				_ => throw new NotSupportedException("Unsupported hash algorithm")
			};

			if (SecretKey != null) {
				HMAC.Key = Base32Encoder.Decode(SecretKey);
			} else {
				SecretKey = Base32Encoder.Encode(HMAC.Key);
			}

			URI = $"otpauth://totp/{Uri.EscapeDataString(Issuer ??= "")}:{Uri.EscapeDataString(Account ??= "")}" +
			      $"?secret={SecretKey}&issuer={Uri.EscapeDataString(Issuer ??= "")}&algorithm={algorithm}&digits={(int)digits}&period={(long)Period}";

			currentCode = GetCode();

			TimeSpan synctoken = TimeSpan.FromMilliseconds(((long)Period * 1000) - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % ((long)Period * 1000));

			UpdateCodeTimer = new Timer(UpdateCode, null, synctoken, TimeSpan.FromSeconds((long)period));
			displayCodeTimer = new Timer(DisplayCode, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

		}

		protected Code GetCode() {
			if (HMAC == null) throw new NullReferenceException("HMAC is null");
			long timeStep = (long)Period;
			long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			long timeCounter = unixTimestamp / timeStep;
			long timeRemaining = (timeStep - unixTimestamp % timeStep) % timeStep;


			byte[] counterBytes = BitConverter.GetBytes(timeCounter);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(counterBytes);
			
			byte[] hash = HMAC.ComputeHash(counterBytes);

			int offset = hash[^1] & 0x0F;

			int binary = (hash[offset] & 0x7F) << 24 |
			             (hash[offset + 1] & 0xFF) << 16 |
			             (hash[offset + 2] & 0xFF) << 8 |
			             (hash[offset + 3] & 0xFF);

			int otp = binary % (int)Math.Pow(10, (int)Digits);


			return new Code(otp.ToString($"D{(int)Digits}"), timeRemaining == 0 ? timeStep : timeRemaining); 
		}

		protected void GenerateQrCode() {
			Utils.GenerateQrCode(URI);
		}

		protected static TOTP? DecodeQrCode(string path) {
			try {
				//otpauth://totp/{account}?secret={secretKey}&issuer={issuer}&algorithm={algorithm}&digits={ndigits}&period={period}
				//otpauth://totp/{Issuer}:{Account"}?secret={SecretKey}&issuer=Issuer}&algorithm={algorithm}&digits={digits}&period={Period}
				var uristring = Utils.DecodeQrCode(path).Text;
				var uri = new Uri(uristring);
				var query = uri.Query;

				var queryParams = System.Web.HttpUtility.ParseQueryString(query);

				var account = uristring.Replace("otpauth://totp/", "").Split("?")[0];
				var secretKey = queryParams.Get("secret");
				var issuer = queryParams.Get("issuer");
				var algorithm = queryParams.Get("algorithm");
				var digits = queryParams.Get("digits");
				var period = queryParams.Get("period");

				algorithm ??= "SHA1";
				digits ??= "6";
				period ??= "30";

				if (issuer == null) {
					issuer = account.Split(':')[0];
					account = account.Split(':')[1];
				} else if (account.Contains(':')) {
					account = account.Split(':')[1];
				}

				return new TOTP((HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), algorithm), secretKey, (Period)Enum.Parse(typeof(Period), period), (Digits)Enum.Parse(typeof(Digits), digits), issuer, account);
			}
			catch (ReaderException e) {
				Console.WriteLine(e.Message);
				return null;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				return null;
			}
		}

	}
}
