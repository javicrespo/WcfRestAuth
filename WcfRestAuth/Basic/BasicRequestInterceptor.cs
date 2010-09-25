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
using WcfRestAuth.Wsse;

namespace WcfRestAuth.Basic
{
    /// <summary>
    /// Basic request interceptor
    /// </summary>
    public class BasicRequestInterceptor : RequestInterceptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicRequestInterceptor"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="realm">The realm.</param>
        public BasicRequestInterceptor(IAuthenticationService authenticationService, string realm)
            : base(false)
        {
            AuthenticationService = authenticationService;
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
        protected IAuthenticationService AuthenticationService
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
            var httpRequest = requestContext.RequestMessage.GetHttpRequestMessage();
            var basicToken = ExtractToken(httpRequest);
            if (basicToken != null && AuthenticateUser(basicToken))
            {
                SecurityContextManager.InitializeSecurityContext(requestContext.RequestMessage,
                                                                                    basicToken.Username);
            }
            else
            {
                RequestContextUtils.Unauthorized(ref requestContext,
                                                  String.Format("Basic realm=\"{0}\"", Realm));
            }
        }

        private bool AuthenticateUser(BasicToken basicToken)
        {
            return AuthenticationService.Validate(basicToken.Username, basicToken.Password);
        }

        private static BasicToken ExtractToken(HttpRequestMessageProperty request)
        {
            var authHeader = request.Headers[HttpRequestHeader.Authorization];

            if (authHeader != null && authHeader.StartsWith("Basic"))
            {
                return CreateBasicToken(authHeader);
            }

            return null;
        }

        private static BasicToken CreateBasicToken(string authHeader)
        {
            var encodedUserPass = authHeader.Substring(6).Trim();

            var encoding = Encoding.GetEncoding(Constants.Enconding);
            var userPass = encoding.GetString(Convert.FromBase64String(encodedUserPass));
            var separator = userPass.IndexOf(':');

            //If there is no : or user or password are empty -> not authorized
            if (separator <= 0 || separator + 1 == userPass.Length)
                return null;

            return new BasicToken
                             {
                                 Username = userPass.Substring(0, separator),
                                 Password = userPass.Substring(separator + 1)
                             };
        }




    }
}
