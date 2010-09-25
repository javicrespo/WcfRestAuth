using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfRestAuth.Digest
{
    public interface ISessionStore
    {
        object Get(string key);
        void Save(string key, object value, TimeSpan sessionTimeout);
    }
}
