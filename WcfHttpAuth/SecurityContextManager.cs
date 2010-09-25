using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;

namespace WcfHttpAuth
{
    internal class SecurityContextManager
    {
        public static void InitializeSecurityContext(Message request, string username)
        {
            var principal = new GenericPrincipal(new GenericIdentity(username), new string[] { });

            var policies = new List<IAuthorizationPolicy> { new PrincipalAuthorizationPolicy(principal) };
            var securityContext = new ServiceSecurityContext(policies.AsReadOnly());

            if (request.Properties.Security != null)
            {
                request.Properties.Security.ServiceSecurityContext = securityContext;
            }
            else
            {
                request.Properties.Security = new SecurityMessageProperty() { ServiceSecurityContext = securityContext };
            }
        }
    }
}
