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

namespace WcfHttpAuth.OAuth
{
    /// <summary>
    /// OAuth 2 legged interceptor
    /// </summary>
    public class Oauth2LeggedInterceptor : RequestInterceptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Oauth2LeggedInterceptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="realm">The realm.</param>
        public Oauth2LeggedInterceptor(IPasswordProvider provider, string realm)
            : this(provider, realm, new TimestampRangeValidator(), new InMemoryNonceStore())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oauth2LeggedInterceptor"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="realm">The realm.</param>
        /// <param name="timestampRangevalidator">The timestamp rangevalidator.</param>
        /// <param name="nonceStore">The nonce store.</param>
        public Oauth2LeggedInterceptor(IPasswordProvider provider, string realm, ITimestampRangeValidator timestampRangevalidator, INonceStore nonceStore)
            : base(false)
        {
            TimestampRangeValidator = timestampRangevalidator;
            NonceStore = nonceStore;
            Provider = provider;
            Realm = realm;
        }

        protected INonceStore NonceStore
        {
            get;
            private set;
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
                var oauthToken = ExtractToken(httpRequest);
                if (oauthToken != null) Unauthorized(ref requestContext);

                if (!IsVersionSupported(oauthToken) || IsSignatureMethodSupported(oauthToken))
                {
                    RequestContextUtils.NotImplemented(ref requestContext);
                    return;
                }

                if (TimestampRangeValidator.ValidateTimestamp(oauthToken.TimestampUtc) &&
                    NonceStore.StoreNonceAndCheckIfItIsUnique(oauthToken.Nonce) &&
                    AuthenticateConsumer(oauthToken))
                {

                    SecurityContextManager.InitializeSecurityContext(requestContext.RequestMessage, oauthToken.ConsumerKey);
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

        private static bool IsSignatureMethodSupported(OAuthToken oauthToken)
        {
            return oauthToken.SignatureMethod != SignatureMethod.HMAC_SHA1;
        }

        private static bool IsVersionSupported(OAuthToken oauthToken)
        {
            return oauthToken.Version == "1.0";
        }

        private void Unauthorized(ref RequestContext requestContext)
        {
            RequestContextUtils.Unauthorized(ref requestContext, String.Format("OAuth realm=\"{0}\"", Realm));
        }

        private bool AuthenticateConsumer(OAuthToken oauthToken)
        {
            var password = Provider.RetrievePassword(oauthToken.ConsumerKey);
            return oauthToken.Verify(password);
        }

        private static OAuthToken ExtractToken(HttpRequestMessageProperty request)
        {
            var authHeader = request.Headers[HttpRequestHeader.Authorization];


            if (authHeader != null && authHeader.StartsWith("OAuth"))
            {
                return CreateOAuthToken(authHeader);
            }

            return null;
        }

        private static OAuthToken CreateOAuthToken(string oauthHeader)
        {
            var result = new OAuthToken();
            var match = new Regex("oauth_consumer_key=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.ConsumerKey = match.Groups["v"].Value;

            match = new Regex("realm=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Realm = match.Groups["v"].Value;

            match = new Regex("oauth_nonce=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Nonce = match.Groups["v"].Value;

            match = new Regex("oauth_timestamp=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Timestamp = match.Groups["v"].Value;

            match = new Regex("oauth_version=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Version = match.Groups["v"].Value;

            match = new Regex("oauth_signature=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.Signature = match.Groups["v"].Value;

            match = new Regex("oauth_signature_method=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(oauthHeader);
            if (!match.Groups["v"].Success)
                return null;
            result.ParseAndSetSignatureMethod(match.Groups["v"].Value);

            return result;
        }






    }
}
