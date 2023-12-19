using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace hOTP {

	public enum HashAlgorithm {
		SHA1,
		SHA256,
		SHA512
	}

	public static class Utils {

		public static byte[] GenerateRandomBytes(int length) {
			byte[] bytes = new byte[length];
			RandomNumberGenerator.Create().GetBytes(bytes);
			return bytes;
		}

		public static void GenerateQrCode(string data) {
			BarcodeWriter barcodeWriter = new BarcodeWriter {
				Format = BarcodeFormat.QR_CODE,
				Options = new ZXing.Common.EncodingOptions {
					Width = 300,
					Height = 300
				}
			};

			Bitmap qrCodeBitmap = barcodeWriter.Write(data);

			qrCodeBitmap.Save("qrcode.png");
		}

		public static Result DecodeQrCode(string path) {
			Bitmap image;
			try {
				image = (Bitmap)Bitmap.FromFile(path);
			}
			catch (Exception) {
				throw new FileNotFoundException("Resource not found: " + path);
			}

			using (image) {
				LuminanceSource source = new BitmapLuminanceSource(image);
				BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
				Result result = new MultiFormatReader().decode(bitmap);
				if (result == null) {
					throw new ReaderException("QR Code not found");
				}
				return result;
			}
		}

	}

	public static class Base32Encoder {
		private const string Base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

		public static string Encode(byte[] data) {
			StringBuilder result = new StringBuilder();

			int bitCount = 0;
			int currentByte = 0;

			foreach (byte b in data) {
				currentByte = (currentByte << 8) | b;
				bitCount += 8;

				while (bitCount >= 5) {
					bitCount -= 5;
					int index = (currentByte >> bitCount) & 0x1F;
					result.Append(Base32Chars[index]);
				}
			}

			if (bitCount > 0) {
				int index = (currentByte << (5 - bitCount)) & 0x1F;
				result.Append(Base32Chars[index]);
			}

			return result.ToString();
		}

		public static byte[] Decode(string base32String) {
			base32String = base32String.ToUpper();

			int bitCount = 0;
			int currentByte = 0;

			using (MemoryStream stream = new MemoryStream()) {
				foreach (char c in base32String) {
					int index = Base32Chars.IndexOf(c);
					if (index == -1) {
						throw new ArgumentException("Invalid character in Base32 string");
					}

					currentByte = (currentByte << 5) | index;
					bitCount += 5;

					if (bitCount >= 8) {
						bitCount -= 8;
						stream.WriteByte((byte)(currentByte >> bitCount));
					}
				}

				return stream.ToArray();
			}
		}
	}

}
