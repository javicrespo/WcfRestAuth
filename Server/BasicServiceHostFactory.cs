using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using WcfHttpAuth.Basic;
using WcfHttpAuth.Wsse;

namespace Server
{
    public class BasicServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var result = new WebServiceHost2(serviceType, true, baseAddresses);

            result.Interceptors.Add(new BasicRequestInterceptor(
                new AuthenticationServiceStub("javi", "password"), "http://tst.com"));
            return result;
        }
    }

   
}