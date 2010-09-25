using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace WcfHttpAuth.Digest
{
    public class AspNetCacheSessionStore:ISessionStore
    {

        public object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }

        public void Save(string key, object value, TimeSpan sessionTimeout)
        {
            HttpRuntime.Cache.Insert(key, value, null, DateTime.MaxValue, sessionTimeout,
                                         CacheItemPriority.High, null);
        }
    }
}
