using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace GrapheneRpc
{
	public static class AES
	{
		public static string ByteArrayToHexString(byte[] ba)
		{
			return BitConverter.ToString(ba).Replace("-", "");
		}

		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}

		public static int AesDecrypt(Byte[] inputBytes, byte[] key, byte[] iv)
		{
			Byte[] outputBytes = inputBytes;

			using (MemoryStream memoryStream = new MemoryStream(outputBytes))
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, GetCryptoAlgorithm().CreateDecryptor(key, iv), CryptoStreamMode.Read))
				{
					return cryptoStream.Read(outputBytes, 0, outputBytes.Length);
				}
			}
		}

		public static byte[] AesEncrypt(string inputText, byte[] key, byte[] iv)
		{
			byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(inputText);

			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, GetCryptoAlgorithm().CreateEncryptor(key, iv), CryptoStreamMode.Write))
				{
					cryptoStream.Write(inputBytes, 0, inputBytes.Length);
					cryptoStream.FlushFinalBlock();

					result = memoryStream.ToArray();
				}
			}

			return result;
		}


		private static RijndaelManaged GetCryptoAlgorithm()
		{
			RijndaelManaged algorithm = new RijndaelManaged();
			//set the mode, padding and block size
			algorithm.Padding = PaddingMode.PKCS7;
			algorithm.Mode = CipherMode.CBC;
			algorithm.KeySize = 128;
			algorithm.BlockSize = 128;
			return algorithm;
		}
	}
}
