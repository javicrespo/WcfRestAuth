using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfRestAuth.Digest
{
    public interface IDigestSessionManager
    {
        void Add(string serverNonce, string opaque);

        bool CheckIfServerNonceAndOpaqueExistAndStoreClientNonceIfNotExists(string serverNonce, string opaque, string clientNonce);

    }
}
