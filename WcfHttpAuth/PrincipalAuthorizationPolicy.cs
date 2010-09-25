using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;

namespace WcfHttpAuth
{
    internal class PrincipalAuthorizationPolicy : IAuthorizationPolicy
    {
        private readonly string id = Guid.NewGuid().ToString();
        private readonly IPrincipal user;

        public PrincipalAuthorizationPolicy(IPrincipal user)
        {
            this.user = user;
        }

        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        public string Id
        {
            get { return id; }
        }

        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            evaluationContext.AddClaimSet(this, new DefaultClaimSet(Claim.CreateNameClaim(user.Identity.Name)));
            evaluationContext.Properties["Identities"] = new List<IIdentity>(new [] { user.Identity });
            evaluationContext.Properties["Principal"] = user;
            return true;
        }

        
    }
}
