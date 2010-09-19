using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Net;
using System.Text.RegularExpressions;
using WcfHttpAuth.Wsse;

namespace WcfHttpAuth.Digest
{
    /// <summary>
    /// Digest request interceptor
    /// </summary>
    public class DigestRequestInterceptor : RequestInterceptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestRequestInterceptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="realm">The realm.</param>
        public DigestRequestInterceptor(IServerDigestProvider provider, string realm)
            : base(false)
        {
            Provider = provider;
            Realm = realm;
        }

        protected string Realm
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        protected IServerDigestProvider Provider
        {
            get;
            private set;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        public override void ProcessRequest(ref RequestContext requestContext)
        {
            try
            {
                var httpRequest = requestContext.RequestMessage.GetHttpRequestMessage();
                var digestToken = ExtractToken(httpRequest);
                if (digestToken != null && AuthenticateUser(digestToken, httpRequest.Method))
                {
                    SecurityContextManager.InitializeSecurityContext(requestContext.RequestMessage,
                                                                                        digestToken.Username);
                }
                else
                {
                    Unauthorized(ref requestContext);
                }
            }
            catch
            {
                Unauthorized(ref requestContext);
            }
        }

        private void Unauthorized(ref RequestContext requestContext)
        {
            RequestContextUtils.Unauthorized(ref requestContext,
                            String.Format("Digest realm=\"{0}\",qop=\"{1}\",nonce=\"{2}\",opaque=\"{3}\"",
                                                    Realm, "auth", Guid.NewGuid(), Guid.NewGuid()));
            requestContext = null;
        }

        private bool AuthenticateUser(DigestToken token, string httpMethod)
        {
            return token.Verify(Provider.RetrieveServerDigest(token.Username), httpMethod);
        }

        private static DigestToken ExtractToken(HttpRequestMessageProperty request)
        {
            var authHeader = request.Headers[Constants.AuthorizationHeader];

            if (authHeader != null && authHeader.StartsWith("Digest"))
            {
                return CreateDigestToken(authHeader);
            }

            return null;
        }

        private static DigestToken CreateDigestToken(string digestHeader)
        {
            var result = new DigestToken();
            var match = new Regex("username=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Username = match.Groups["v"].Value;

            match = new Regex("nonce=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Nonce = match.Groups["v"].Value;

            match = new Regex("uri=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Path = match.Groups["v"].Value;

            match = new Regex(@"nc=(?<v>\d+)", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.SequenceNumber = match.Groups["v"].Value;

            match = new Regex("cnonce=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.ClientNonce = match.Groups["v"].Value;

            match = new Regex("response=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(digestHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Digest = match.Groups["v"].Value;


            return result;
        }
    }
}
