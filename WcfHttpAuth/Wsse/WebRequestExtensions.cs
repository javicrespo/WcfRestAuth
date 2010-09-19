using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace WcfHttpAuth.Wsse
{
    public static class WebRequestExtensions
    {
        public static WebRequest WithWsseToken(this WebRequest request, string userName, string password)
        {
            var wsseToken = new WsseToken 
            { 
                Username = userName, 
                Created = UtcUtils.UtcNowString(), 
                Nonce = Guid.NewGuid().ToString()
            };

            return WithWsseToken(request, wsseToken, password);
        }

        internal static WebRequest WithWsseToken(this WebRequest request, WsseToken wsseToken, string password)
        {
            wsseToken.Calculate(password);

            request.Headers.Add(Constants.AuthorizationHeader, "WSSE profile=\"UsernameToken\"");
            request.Headers.Add("X-WSSE", string.Format("UsernameToken UserName=\"{0}\", Created=\"{1}\", Nonce=\"{2}\", PasswordDigest=\"{3}\"",
                                                                    wsseToken.Username, wsseToken.Created, wsseToken.Nonce, wsseToken.PasswordDigest));
            return request;
        }
    }
}
