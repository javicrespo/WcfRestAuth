using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Digest
{
    public interface IDigestSessionStore
    {
        void Add(string serverNonce, string opaque);

        bool CheckIfServerNonceAndOpaqueExistAndStoreClientNonceIfNotExists(string p, string p_2, string p_3);
    }
}
