using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Globalization;

namespace WcfHttpAuth.Wsse
{
    internal class WsseToken
    {
        public string Username { get; set; }
        public string PasswordDigest { get; set; }
        public string Created { get; set; }
        public DateTime CreatedDate
        {
            get 
            {
                return UtcUtils.FromUtcString(Created);
            }
        }
        public string Nonce { get; set; }

        public void Calculate(string password)
        {
            PasswordDigest = CalculateDigest(password);
        }

        public bool Verify(string password)
        {
            return PasswordDigest == CalculateDigest(password);
        }

        private string CalculateDigest(string password)
        {
            var text = string.Format("{0}{1}{2}", Nonce, Created, password);
            var encoding = Encoding.GetEncoding(Constants.Enconding);
            var digest = new SHA1CryptoServiceProvider().ComputeHash(encoding.GetBytes(text));
            return Convert.ToBase64String(digest);
        }
    }
}
