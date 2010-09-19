using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace WcfHttpAuth.IntegrationTests
{
    [TestClass]
    public class When_calling_a_basic_auth_service
    {
        [TestMethod]
        public void Given_credentials_are_invalid_then_should_return_401()
        {
            var request = WebRequest.Create("http://localhost:2391/BasicService.svc/HelloWorld");
            request.Credentials = new NetworkCredential("user", "fakepassword");
            try
            {
                using (var readStream = new StreamReader(request.GetResponse().GetResponseStream()))
                {

                }
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
            }
        }

        [TestMethod]
        public void Given_credentials_are_valid_then_should_return_200()
        {
            var request = WebRequest.Create("http://localhost:2391/BasicService.svc/HelloWorld");
            request.Credentials = new NetworkCredential("user", "password");

            using (var readStream = new StreamReader(request.GetResponse().GetResponseStream()))
            {

            }
        }
    }
}
