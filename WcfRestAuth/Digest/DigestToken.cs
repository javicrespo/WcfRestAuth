using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using WcfRestAuth.Wsse;

namespace WcfRestAuth.Digest
{
    internal class DigestToken
    {
        public string Username { get; set; }

        public string Digest { get; set; }

        public string ClientNonce { get; set; }

        public string ServerNonce { get; set; }

        public string SequenceNumber { get; set; }

        public string Path { get; set; }

        public string Opaque { get; set; }


        public bool Verify(string userDigest, string httpMethod)
        {
            return Digest == CalculateDigestFromUserDigest(userDigest, httpMethod);
        }

        public void CalculateDigest(string realm, string username, string password, string httpMethod)
        {

            Digest = CalculateDigestFromUserDigest(DigestUtils.GenerateUserDigest(realm, username, password), httpMethod);
        }

        private string CalculateDigestFromUserDigest(string userDigest, string httpMethod)
        {
            var text1 = string.Format("{0}:{1}", httpMethod, Path);
            var encoding = Encoding.GetEncoding(Constants.Enconding);
            var md5 = new MD5CryptoServiceProvider();
            var digest1 = DigestUtils.ToHexString(md5.ComputeHash(encoding.GetBytes(text1)));

            var text2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                                        userDigest, ServerNonce, SequenceNumber, ClientNonce, "auth", digest1);
            return DigestUtils.ToHexString(md5.ComputeHash(encoding.GetBytes(text2)));
        }



       
    }
}
