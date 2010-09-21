using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace WcfHttpAuth.Digest
{
    internal class AspNetCacheSessionStore : IDigestSessionStore
    {
        private TimeSpan sessionTimeout;

        public AspNetCacheSessionStore()
            : this(new TimeSpan(0, 20, 0))
        {

        }

        public AspNetCacheSessionStore(TimeSpan sessionTimeout)
        {
            this.sessionTimeout = sessionTimeout;
        }

        public void Add(string serverNonce, string opaque)
        {
            HttpRuntime.Cache.Insert(serverNonce, new DigestSession(opaque), null, DateTime.MaxValue, sessionTimeout,
                                        CacheItemPriority.High, null);
        }

        public bool CheckIfServerNonceAndOpaqueExistAndStoreClientNonceIfNotExists(string serverNonce, string opaque, string clientNonce)
        {
            var session = HttpRuntime.Cache[serverNonce] as DigestSession;
            if (session == null)
                return false;
            if (session.Opaque != opaque)
                return false;
            lock (session)
            {
                if (session.ClientNonces.Contains(clientNonce))
                    return false;
                session.ClientNonces.Add(clientNonce);
            }
            return true;
        }

        class DigestSession
        {
            public DigestSession(string opaque)
            {
                Opaque = opaque;
                ClientNonces = new List<string>();
            }

            public string Opaque { get; private set; }
            public List<string> ClientNonces { get; private set; }
        }
    }
}
