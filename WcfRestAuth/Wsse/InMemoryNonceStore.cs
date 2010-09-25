using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfRestAuth.Wsse
{
    /// <summary>
    /// In memory nonce store implementation
    /// </summary>
    public class InMemoryNonceStore:INonceStore
    {
        private int maxNumberOfNonces = 5000;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryNonceStore"/> class.
        /// </summary>
        public InMemoryNonceStore()
            :this(5000)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryNonceStore"/> class.
        /// </summary>
        /// <param name="maxNumberOfNonces">The max number of nonces.</param>
        public InMemoryNonceStore(int maxNumberOfNonces)
        {
            this.maxNumberOfNonces = maxNumberOfNonces;
        }
       
        private List<string> nonces = new List<string>();


        /// <summary>
        /// Stores the nonce and checks if it is unique.
        /// </summary>
        /// <param name="nonce">The nonce.</param>
        /// <returns>If the store it's unique</returns>
        public bool StoreNonceAndCheckIfItIsUnique(string nonce)
        {
            if (!nonces.Contains(nonce))
            {
                lock (nonces)
                {
                    if (nonces.Count >= maxNumberOfNonces)
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
