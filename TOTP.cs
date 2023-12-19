using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace hOTP {
	public class TOTP {
		public HashAlgorithm Algorithm { get; }
		public long TimeRemaining { get; private set; }
		public long Period { get; }
		public int NumberOfDigits { get; }
		public string? Account { get; }
		public string? Issuer { get; }
		public string? SecretKey { get; }
		public string URI { get; }
		
		private HMAC? HMAC { get; }


		public TOTP(HashAlgorithm algorithm = HashAlgorithm.SHA256, string? secretKey = null, long period = 30, int numberOfDigits = 6, string? issuer = null, string? account = null) {
			Algorithm = algorithm;
			Period = period;
			NumberOfDigits = numberOfDigits;
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
			      $"?secret={SecretKey}&issuer={Uri.EscapeDataString(Issuer ??= "")}&algorithm={algorithm}&digits={numberOfDigits}&period={Period}";
		}

		public Code GetCode() {
			if (HMAC == null) throw new NullReferenceException("HMAC is null");
			long timeStep = Period;
			long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			long timeCounter = unixTimestamp / timeStep;

			TimeRemaining = (timeStep - unixTimestamp % timeStep) % timeStep;

			byte[] counterBytes = BitConverter.GetBytes(timeCounter);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(counterBytes);
			
			byte[] hash = HMAC.ComputeHash(counterBytes);

			int offset = hash[^1] & 0x0F;

			int binary = (hash[offset] & 0x7F) << 24 |
			             (hash[offset + 1] & 0xFF) << 16 |
			             (hash[offset + 2] & 0xFF) << 8 |
			             (hash[offset + 3] & 0xFF);

			int otp = binary % (int)Math.Pow(10, NumberOfDigits);


			return new Code(otp.ToString($"D{NumberOfDigits}"), TimeRemaining == 0 ? timeStep : TimeRemaining); 
		}

		public void GenerateQrCode() {
			Utils.GenerateQrCode(URI);
		}

		public static TOTP? DecodeQrCode(string path) {
			try {
				var uri = Utils.DecodeQrCode(path).Text;

				uri = uri.Replace("otpauth://totp/", "");

				var issuer = uri.Split(":")[0];
				var account = uri.Split(":")[1].Split("?")[0];

				var secretKey = uri.Split("secret=")[1].Split("&")[0];
				var algorithm = uri.Split("algorithm=")[1].Split("&")[0];
				var digits = uri.Split("digits=")[1].Split("&")[0];
				var period = uri.Split("period=")[1].Split("&")[0];

				return new TOTP((HashAlgorithm)Enum.Parse(typeof(HashAlgorithm), algorithm), secretKey, long.Parse(period), int.Parse(digits), issuer, account);
			}
			catch (ReaderException e) {
				Console.WriteLine(e.Message);
				return null;
			}
		}

	}
}
