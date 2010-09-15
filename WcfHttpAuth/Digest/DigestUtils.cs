using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WcfHttpAuth.Digest
{
    public static class DigestUtils
    {
        public static string GenerateServerDigest(string realm, string username, string password)
        {
            var text1 = string.Format("{0}:{1}:{2}", username, realm, password);
            var encoding = Encoding.GetEncoding(Constants.Enconding);
            var md5 = new MD5CryptoServiceProvider();
            return ToHexString(md5.ComputeHash(encoding.GetBytes(text1)));
        }

        internal static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
