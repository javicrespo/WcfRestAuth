using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace WcfRestAuth.IntegrationTests
{
    [TestClass]
    public class When_calling_a_basic_auth_service
    {
        private const string ServiceUrl = "http://localhost:2391/BasicService.svc/HelloWorld";

        [TestMethod]
        public void Given_credentials_are_invalid_then_should_return_401()
        {
            var request = WebRequest.Create(ServiceUrl);
            request.Credentials = new NetworkCredential(Constants.Username, Constants.BadPassword);
            try
            {
                using (request.GetResponse())
                {

                }
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }

        [TestMethod]
        public void Given_credentials_are_valid_then_should_return_200()
        {
            var request = WebRequest.Create(ServiceUrl);
            request.Credentials = new NetworkCredential(Constants.Username, Constants.Password);

            using (request.GetResponse())
            {

            }
        }
    }
}
