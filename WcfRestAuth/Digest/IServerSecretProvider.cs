using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfRestAuth.Digest
{
    public interface IServerSecretProvider
    {
        string RetrieveServerSecret(string username);
    }
}
