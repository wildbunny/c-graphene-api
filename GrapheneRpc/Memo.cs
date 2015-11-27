using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Casascius.Bitcoin;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Math.EC;
using BitsharesCore;

namespace GrapheneRpc
{
	public class Memo
	{

		/// <summary>	Gets shared secret. </summary>
		///
		/// def get_shared_secret(priv, pub) :
		///    pub_point  = pub.point()
		///    priv_point = int(repr(priv),16)
		///    res     = pub_point * priv_point
		///    res_hex = '%032x' % res.x()
		///    return res_hex
		///    
		/// <remarks>	Paul, 26/10/2015. </remarks>
		///
		/// <returns>	The shared secret. </returns>
		static public byte[] GetSharedSecret(KeyPair priv, PublicKey pub)
		{
			var ps = SecNamedCurves.GetByName("secp256k1");

			BigInteger ipriv = new BigInteger(1, priv.PrivateKeyBytes);
			ECPoint pubPoint = pub.GetECPoint();

			ECPoint shared = pubPoint.Multiply(ipriv);

			BigInteger s = shared.X.ToBigInteger();

			byte[] data = s.ToByteArray();

			int leadingZeros=-1;
			while(data[++leadingZeros] == 0);

			byte[] result = new byte[data.Length - leadingZeros];
			Buffer.BlockCopy(data, leadingZeros, result, 0, result.Length);

			return result;
		}

		/// <summary>	Decrypts./ </summary>
		///
		///		auto secret = priv.get_shared_secret(pub);
		///      auto nonce_plus_secret = fc::sha512::hash(fc::to_string(nonce) + secret.str());
		///      auto plain_text = fc::aes_decrypt( nonce_plus_secret, message );
		///      auto result = memo_message::deserialize(string(plain_text.begin(), plain_text.end()));
		///      FC_ASSERT( result.checksum == uint32_t(digest_type::hash(result.text)._hash[0]) );
		///      return result.text;
		/// 
		/// <remarks>	Paul, 26/10/2015. </remarks>
		///
		/// <param name="memo">				 	The memo. </param>
		/// <param name="receiverPrivateKey">	The receiver private key. </param>
		/// <param name="senderPublicKey">   	The sender public key. </param>
		///
		/// <returns>	A string. </returns>
		static public string Decrypt(GrapheneMemo memo, KeyPair receiverKeyPair)
		{
			PublicKey pub = new BitsharesPubKey(memo.from).GetBitcoinPubKey();
			byte[] sharedSecret = GetSharedSecret(receiverKeyPair, pub);

			string ss = StringExtensions.ByteArrayToHexString(sharedSecret, true);
			
			string ps = memo.nonce.ToString() + StringExtensions.ByteArrayToHexString(Crypto.ComputeSha512( sharedSecret), true);

			byte[] seed = ASCIIEncoding.ASCII.GetBytes(ps);

			string hex = StringExtensions.ByteArrayToHexString(Crypto.ComputeSha512(seed), true);

			string key = hex.Substring(0, 64);
			string iv = hex.Substring(64, 32);

			byte[] data = Util.HexStringToBytes(memo.message);

			int len = AES.AesDecrypt(data, Util.HexStringToBytes(key), Util.HexStringToBytes(iv));

			byte[] result = new byte[len - 4];
			Buffer.BlockCopy(data, 4, result, 0, result.Length);

			string message = UTF8Encoding.UTF8.GetString(result);
			
			return message;
		}
	}
}
