using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth
{
    public interface INonceCache
    {
        bool StoreNonceAndCheckIfItIsUnique(string nonce);
    }
}
