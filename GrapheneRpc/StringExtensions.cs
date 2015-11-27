using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrapheneRpc
{
	static public class StringExtensions
	{
		public static string TrimStart(this string source, string value)
		{
			if (!source.StartsWith(value))
				return source;

			return source.Remove(source.IndexOf(value), value.Length);
		}

		public static string TrimEnd(this string source, string value)
		{
			if (!source.EndsWith(value))
				return source;

			return source.Remove(source.LastIndexOf(value));
		}

		/// <summary>	Hexadecimal string to byte array. </summary>
		///
		/// <remarks>	Paul, 03/08/2015. </remarks>
		///
		/// <param name="hex">	The hexadecimal. </param>
		///
		/// <returns>	A byte[]. </returns>
		public static byte[] HexStringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}

		/// <summary>	Byte array to hexadecimal string. </summary>
		///
		/// <remarks>	Paul, 03/08/2015. </remarks>
		///
		/// <param name="data">	The data. </param>
		///
		/// <returns>	A string. </returns>
		static public string ByteArrayToHexString(byte[] data, bool toLower = false)
		{
			string s = string.Concat(data.Select(b => b.ToString("X2")).ToArray());
			if (toLower)
			{
				return s.ToLower();
			}
			else
			{
				return s;
			}
		}
	}
}
