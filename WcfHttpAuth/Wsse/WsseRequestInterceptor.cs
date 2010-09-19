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

namespace WcfHttpAuth.Wsse
{
    /// <summary>
    /// WSSE request interceptor
    /// </summary>
    public class WsseRequestInterceptor : RequestInterceptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="WsseRequestInterceptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="realm">The realm.</param>
        public WsseRequestInterceptor(IPasswordProvider provider, string realm)
            : base(false)
        {
            Provider = provider;
            Realm = realm;
            TimestampRangeValidator = new TimestampRangeValidator();
        }

        public WsseRequestInterceptor(IPasswordProvider provider, string realm, ITimestampRangeValidator timestampRangevalidator)
            :this(provider, realm)
        {
            TimestampRangeValidator = timestampRangevalidator;
        }

        protected ITimestampRangeValidator TimestampRangeValidator
        {
            get;
            private set;
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
        protected IPasswordProvider Provider
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
                var wsseToken = ExtractToken(httpRequest);
                if (wsseToken != null && 
                    AuthenticateUser(wsseToken) && 
                    TimestampRangeValidator.ValidateTimestamp(wsseToken.CreatedDate))
                {

                    SecurityContextManager.InitializeSecurityContext(requestContext.RequestMessage,
                                                                                        wsseToken.Username);
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
                                                         String.Format("WSSE realm=\"{0}\", profile=\"UsernameToken\"",
                                                                        Realm));
        }

        private bool AuthenticateUser(WsseToken wsseToken)
        {
            var password = Provider.RetrievePassword(wsseToken.Username);
            return wsseToken.Verify(password);
        }

        private static WsseToken ExtractToken(HttpRequestMessageProperty request)
        {
            var authHeader = request.Headers[HttpRequestHeader.Authorization];
            var wsseHeader = request.Headers["X-WSSE"];

            if (authHeader != null && authHeader.StartsWith("WSSE profile=\"UsernameToken\"") &&
                wsseHeader != null && wsseHeader.StartsWith("UsernameToken"))
            {
                return CreateWsseToken(wsseHeader);
            }

            return null;
        }

        private static WsseToken CreateWsseToken(string wsseHeader)
        {
            var result = new WsseToken();
            var match = new Regex("UserName=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(wsseHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Username = match.Groups["v"].Value;

            match = new Regex("Created=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(wsseHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Created = match.Groups["v"].Value;

            match = new Regex("Nonce=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(wsseHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Nonce = match.Groups["v"].Value;

            match = new Regex("PasswordDigest=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(wsseHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.PasswordDigest = match.Groups["v"].Value;

            return result;
        }

        

       
    }
}
