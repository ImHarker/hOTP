using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace hOTPwin {
	public class TokenManager {

		public ObservableCollection<TOTPwin> Tokens { get; set; } = new ObservableCollection<TOTPwin>();
		public EncryptionManager EncryptionManager { get; set; } = new EncryptionManager();

		public void ExportTokens() {
			var uriList = Tokens.Select(t => t.URI).ToList();
			var data = JsonSerializer.Serialize(uriList);
			var encryptedData = EncryptionManager.Encrypt(data, EncryptionManager.Key);
			System.IO.File.WriteAllBytes("tokens.dat", encryptedData);
		}

		public void ImportTokens() {
			if (!System.IO.File.Exists("tokens.dat")) return;
			var encryptedData = System.IO.File.ReadAllBytes("tokens.dat");
			var data = EncryptionManager.Decrypt(encryptedData, EncryptionManager.Key);
			var uriList = JsonSerializer.Deserialize<List<string>>(data);
			foreach (var uri in uriList) {
				var token = TOTPwin.DecodeURI(uri);
				if (token != null)
					Tokens.Add(token);
			}
		}


	}

	public class EncryptionManager {

		public byte[]? Key { get; private set; } = null;
		private byte[]? Salt { get; set; } = null;

		public void PassToKey(string password) {
			byte[]? salt = null;
			
			if (System.IO.File.Exists("tokens.dat")) {
				var encryptedData = System.IO.File.ReadAllBytes("tokens.dat");
				salt = encryptedData.Take(32).ToArray();
			}
			
			salt ??= RandomNumberGenerator.GetBytes(256 / 8);
			var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, 600_000, HashAlgorithmName.SHA256, 256/8);
			Key = key;
			Salt = salt;
		}

		public byte[] Encrypt(string data, byte[] key) {
			AesCng aes = new AesCng();
			aes.GenerateIV();
			aes.Key = key; //KEY -> 256 bits (32 bytes)
			aes.Padding = PaddingMode.PKCS7;
			aes.Mode = CipherMode.CBC;
			var iv = aes.IV;
			var encryptedData = aes.CreateEncryptor().TransformFinalBlock(Encoding.UTF8.GetBytes(data), 0, data.Length);
			encryptedData = iv.Concat(encryptedData).ToArray();
			encryptedData = Salt.Concat(encryptedData).ToArray();
			return encryptedData;
		}

		public string Decrypt(byte[] data, byte[] key) {
			var salt = data.Take(32).ToArray();
			var iv = data.Skip(32).Take(16).ToArray();
			AesCng aes = new AesCng();
			aes.IV = iv;
			aes.Key = key; //KEY -> 256 bits (32 bytes)
			aes.Padding = PaddingMode.PKCS7;
			aes.Mode = CipherMode.CBC;
			var encryptedData = data.Skip(32+16).ToArray();
			var decryptedData = aes.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
			return Encoding.UTF8.GetString(decryptedData);

		}
	}
}
