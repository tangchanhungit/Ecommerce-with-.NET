using System.Security.Cryptography;
using System.Text;

namespace Ecommerce_web_app.Extentions
{
	public static class HashMD5
	{
		public static string ToMD5(this string s) 
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
			StringBuilder sb = new StringBuilder();
			foreach (byte b in hash)
				sb.Append(String.Format("{0:x2}",b));
			return sb.ToString();
		}
	}
}
