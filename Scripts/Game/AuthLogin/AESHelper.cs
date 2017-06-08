using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class AESHelper : MonoBehaviour {

	private static string key = "i2H2m1HuUofKWArw";

	/// <summary>
	/// Encrypt the specified toEncrypt and key.
	/// </summary>
	/// <param name="toEncrypt">To encrypt.</param>
	/// <param name="key">Key.</param>
	public static string Encrypt(string toEncrypt){
		byte[] keyArr = Encoding.UTF8.GetBytes (key);
		byte[] toEncryArr = Encoding.UTF8.GetBytes (toEncrypt);

		RijndaelManaged rDel = new RijndaelManaged();
		rDel.BlockSize = 128;
		rDel.Key = keyArr;
		rDel.Mode = CipherMode.ECB;

		ICryptoTransform cTrans = rDel.CreateEncryptor ();
		byte[] resultArr = cTrans.TransformFinalBlock (toEncryArr, 0, toEncryArr.Length);

		return System.Convert.ToBase64String (resultArr, 0, resultArr.Length);
	}

	/// <summary>
	/// Decrypt the specified toDecrypt and key.
	/// </summary>
	/// <param name="toDecrypt">To decrypt.</param>
	/// <param name="key">Key.</param>
	public static string Decrypt(string toDecrypt){
		byte[] keyArr = UTF8Encoding.UTF8.GetBytes (key.Substring(0, 16));
		byte[] toDecryptArr = System.Convert.FromBase64String (toDecrypt);

		RijndaelManaged rDel = new RijndaelManaged ();
		rDel.Key = keyArr;
		rDel.Mode = CipherMode.ECB;

		ICryptoTransform cTransform = rDel.CreateDecryptor ();
		byte[] resultArr = cTransform.TransformFinalBlock (toDecryptArr, 0, toDecryptArr.Length);

		return UTF8Encoding.UTF8.GetString (resultArr);
	}
}
