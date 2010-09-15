using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Server
{
    [ServiceContract]
    public class DigestService 
    {
        [OperationContract]
        [WebGet]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
