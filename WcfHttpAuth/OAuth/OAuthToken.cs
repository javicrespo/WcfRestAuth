using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.OAuth
{
    internal class OAuthToken
    {
        public string ConsumerKey { get; set; }
        
        public string Realm { get; set; }

        public string Nonce { get; set; }

        public string Version { get; set; }

        public string Signature { get; set; }

        public SignatureMethod SignatureMethod { get; set; }

        public string Timestamp { get; set; }

        public DateTime TimestampUtc
        { 
            get 
            {
                return UtcUtils.FromEpochString(Timestamp);
            } 
        }

        internal void ParseAndSetSignatureMethod(string signatureMethod)
        {
            if (String.Compare(signatureMethod, "PLAINTEXT", StringComparison.OrdinalIgnoreCase) == 0)
            {
                SignatureMethod = OAuth.SignatureMethod.PlainText;
            }
            else if (String.Compare(signatureMethod, "HMAC-SHA1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                SignatureMethod = OAuth.SignatureMethod.HMAC_SHA1;
            }
            else if (String.Compare(signatureMethod, "RSA-SHA1", StringComparison.OrdinalIgnoreCase) == 0)
            {
                SignatureMethod = OAuth.SignatureMethod.RSA_SHA1;
            }
        }

        internal bool Verify(string password)
        {
            return true;
        }

        
    }

    internal enum SignatureMethod
    {
        None = 0,
        PlainText,
        HMAC_SHA1, 
        RSA_SHA1
    }
}
