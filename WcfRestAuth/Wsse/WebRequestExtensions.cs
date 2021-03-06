﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WcfRestAuth.Wsse
{
    /// <summary>
    /// Web request extension methods
    /// </summary>
    public static class WebRequestExtensions
    {
        /// <summary>
        /// Adds Wsse username token to the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public static WebRequest WithWsseToken(this WebRequest request, string userName, string password)
        {
            var wsseToken = new WsseToken 
            { 
                Username = userName, 
                Created = UtcUtils.UtcNowString(), 
                Nonce = NonceGenerator.Generate()
            };

            return WithWsseToken(request, wsseToken, password);
        }

        internal static WebRequest WithWsseToken(this WebRequest request, WsseToken wsseToken, string password)
        {
            wsseToken.Calculate(password);

            request.Headers.Add(HttpRequestHeader.Authorization, "WSSE profile=\"UsernameToken\"");
            request.Headers.Add("X-WSSE", string.Format("UsernameToken UserName=\"{0}\", Created=\"{1}\", Nonce=\"{2}\", PasswordDigest=\"{3}\"",
                                                                    wsseToken.Username, wsseToken.Created, wsseToken.Nonce, wsseToken.PasswordDigest));
            return request;
        }
    }
}
