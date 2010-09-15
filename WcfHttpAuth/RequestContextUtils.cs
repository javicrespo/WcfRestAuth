using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.ServiceModel.Channels;
using WcfHttpAuth.Wsse;

namespace WcfHttpAuth
{
    static class RequestContextUtils
    {
        public static void Unauthorized(ref RequestContext context, string nonauthorizedHeaderValue)
        {
            var reply = Message.CreateMessage(MessageVersion.None, null);
            var responseProperty = new HttpResponseMessageProperty() { StatusCode = HttpStatusCode.Unauthorized };

            responseProperty.Headers.Add(Constants.NonAuthorizedHeader,
                                         nonauthorizedHeaderValue);

            reply.Properties[HttpResponseMessageProperty.Name] = responseProperty;
            context.Reply(reply);
            context = null;
        }

        public static HttpRequestMessageProperty GetHttpRequestMessage(this Message requestMessage)
        {
            return (HttpRequestMessageProperty)requestMessage.Properties[HttpRequestMessageProperty.Name];
        }
    }
}
