using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WcfHttpAuth.Digest
{
    internal class DefaultDigestSessionManager : IDigestSessionManager
    {
        private readonly TimeSpan sessionTimeout;
        private readonly ISessionStore sessionStore;

        public DefaultDigestSessionManager()
            : this(new TimeSpan(0, 20, 0), new AspNetCacheSessionStore())
        {

        }

        public DefaultDigestSessionManager(ISessionStore sessionStore)
            : this(new TimeSpan(0, 20, 0), sessionStore)
        {

        }

        public DefaultDigestSessionManager(TimeSpan sessionTimeout, ISessionStore sessionStore)
        {
            this.sessionTimeout = sessionTimeout;
            this.sessionStore = sessionStore;
        }

        public void Add(string serverNonce, string opaque)
        {
            sessionStore.Save(serverNonce, new DigestSession(opaque), sessionTimeout);
        }

        public bool CheckIfServerNonceAndOpaqueExistAndStoreClientNonceIfNotExists(string serverNonce, string opaque, string clientNonce)
        {
            var session = sessionStore.Get(serverNonce) as DigestSession;
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
    }
    
    internal class DigestSession
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
