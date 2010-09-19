using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth
{
    public class InMemoryNonceStore:INonceStore
    {
        private int maxNumberOfNonces = 5000;

        public int MaxNumberOfNonces
        {
            get { return maxNumberOfNonces; }
            set { maxNumberOfNonces = value; }
        }
       
        private static List<string> nonces = new List<string>();


        public bool StoreNonceAndCheckIfItIsUnique(string nonce)
        {
            if (!nonces.Contains(nonce))
            {
                lock (nonces)
                {
                    if (nonces.Count >= MaxNumberOfNonces)
                    {
                        nonces.RemoveAt(0);
                    }
                    nonces.Add(nonce);
                    return true;
                }
            }
            return false;
        }
    }
}
