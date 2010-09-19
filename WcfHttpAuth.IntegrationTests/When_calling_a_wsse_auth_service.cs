using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using WcfHttpAuth.Wsse;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace WcfHttpAuth.IntegrationTests
{
    [TestClass]
    public class When_calling_a_wsse_auth_service
    {
        [TestMethod]
        //[AspNetDevelopmentServer("localhost:3000", @"..\Server")]
        public void Given_credentials_are_invalid_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create("http://localhost:2391/WsseService.svc/HelloWorld")
                    .WithWsseToken("user", "fakepassword")
                    .GetResponse();

            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
            }
        }

        [TestMethod]
        public void Given_credentials_are_valid_then_should_return_200()
        {

            var response = WebRequest.Create("http://localhost:2391/WsseService.svc/HelloWorld")
                .WithWsseToken("user", "password")
                .GetResponse();
        }

        [TestMethod]
        public void Given_timestamp_is_below_the_approved_threshold_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create("http://localhost:2391/WsseService.svc/HelloWorld")
                    .WithWsseToken(new WsseToken 
                    {
                        Username = "user", 
                        Nonce = NonceGenerator.Generate(),
                        Created = UtcUtils.UtcString(DateTime.UtcNow.AddHours(-2))
                    }, "password")
                    .GetResponse();
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
            }
        }

        [TestMethod]
        public void Given_timestamp_is_above_the_approved_threshold_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create("http://localhost:2391/WsseService.svc/HelloWorld")
                    .WithWsseToken(new WsseToken
                    {
                        Username = "user",
                        Nonce = NonceGenerator.Generate(),
                        Created = UtcUtils.UtcString(DateTime.UtcNow.AddHours(2))
                    }, "password")
                    .GetResponse();
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
            }
        }
    }
}
