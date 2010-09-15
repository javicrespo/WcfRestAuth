using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using WcfHttpAuth.Wsse;

namespace WcfHttpAuth.Digest
{
    internal class DigestToken
    {
        public string Username { get; set; }

        public string Digest { get; set; }

        public string ClientNonce { get; set; }

        public string Nonce { get; set; }

        public string SequenceNumber { get; set; }

        public string Path { get; set; }


        public bool Verify(string serverDigest, string httpMethod)
        {
            return Digest == CalculateDigest(serverDigest, httpMethod);
        }

        private string CalculateDigest(string serverDigest, string httpMethod)
        {
            var text1 = string.Format("{0}:{1}", httpMethod, Path);
            var encoding = Encoding.GetEncoding(Constants.Enconding);
            var md5 = new MD5CryptoServiceProvider();
            var digest1 = BitConverter.ToString(md5.ComputeHash(encoding.GetBytes(text1))).Replace("-", "").ToLower();

            var text2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                        serverDigest, Nonce, SequenceNumber, ClientNonce, "auth", digest1);
            return DigestUtils.ToHexString(md5.ComputeHash(encoding.GetBytes(text2)));
        }
    }
}
