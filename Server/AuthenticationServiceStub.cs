using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WcfRestAuth.Basic;
using WcfRestAuth.Digest;
using WcfRestAuth.Wsse;
using WcfRestAuth;

namespace Server
{
    class AuthenticationServiceStub : IAuthenticationService, IPasswordProvider, IServerSecretProvider
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public AuthenticationServiceStub(string username, string password)
        {
            Password = password;
            Username = username;
        }

        bool IAuthenticationService.Validate(string username, string password)
        {
            return username == Username && password == Password;
        }

        string IPasswordProvider.RetrievePassword(string username)
        {
            return Password;
        }

        public string RetrieveServerSecret(string username)
        {
            return DigestUtils.GenerateUserDigest("http://tst.com", "user", "password");
        }
    }
}
