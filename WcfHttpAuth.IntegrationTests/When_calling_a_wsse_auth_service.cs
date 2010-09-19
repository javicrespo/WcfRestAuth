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
        private const string ServiceUrl = "http://localhost:2391/WsseService.svc/HelloWorld";
        [TestMethod]
        //[AspNetDevelopmentServer("localhost:3000", @"..\Server")]
        public void Given_credentials_are_invalid_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create(ServiceUrl)
                    .WithWsseToken(Constants.Username, Constants.BadPassword)
                    .GetResponse();

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

            var response = WebRequest.Create(ServiceUrl)
                .WithWsseToken(Constants.Username, Constants.Password)
                .GetResponse();
        }

        [TestMethod]
        public void Given_timestamp_is_below_the_approved_threshold_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create(ServiceUrl)
                    .WithWsseToken(new WsseToken
                    {
                        Username = Constants.Username,
                        Nonce = NonceGenerator.Generate(),
                        Created = UtcUtils.UtcString(DateTime.UtcNow.AddHours(-2))
                    }, Constants.Password)
                    .GetResponse();
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }

        [TestMethod]
        public void Given_timestamp_is_above_the_approved_threshold_then_should_return_401()
        {
            try
            {
                var response = WebRequest.Create(ServiceUrl)
                    .WithWsseToken(new WsseToken
                    {
                        Username = Constants.Username,
                        Nonce = NonceGenerator.Generate(),
                        Created = UtcUtils.UtcString(DateTime.UtcNow.AddHours(2))
                    }, Constants.Password)
                    .GetResponse();
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }

        [TestMethod]
        public void Given_message_is_replayed_then_should_return_401()
        {

            var token = new WsseToken
                {
                    Username = Constants.Username,
                    Nonce = NonceGenerator.Generate(),
                    Created = UtcUtils.UtcNowString()
                };
            WebRequest.Create(ServiceUrl)
                .WithWsseToken(token, Constants.Password)
                .GetResponse();
            try
            {
                WebRequest.Create(ServiceUrl)
                .WithWsseToken(token, Constants.Password)
                .GetResponse();
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }
    }
}
