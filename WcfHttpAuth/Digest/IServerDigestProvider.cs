using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfHttpAuth.Digest
{
    public interface IServerDigestProvider
    {
        string RetrieveServerDigest(string username);
    }
}
