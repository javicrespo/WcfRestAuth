using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfRestAuth.Wsse
{
    /// <summary>
    /// Nonce store
    /// </summary>
    public interface INonceStore
    {
        /// <summary>
        /// Stores the nonce and checks if it is unique.
        /// </summary>
        /// <param name="nonce">The nonce.</param>
        /// <returns>If the store it's unique</returns>
        bool StoreNonceAndCheckIfItIsUnique(string nonce);
    }
}
