using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using WcfHttpAuth.Digest;

namespace Server
{
    public class DigestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var result = new WebServiceHost2(serviceType, true, baseAddresses);

            result.Interceptors.Add(
                new DigestRequestInterceptor(new AuthenticationServiceStub("javi","password"), "http://tst.com"));
            return result;
        }
    }
}