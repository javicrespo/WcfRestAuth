using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using WcfHttpAuth.Digest;

namespace WcfHttpAuth.IntegrationTests
{
    [TestClass]
    public class When_calling_a_digest_auth_service
    {
        private const string ServiceUrl = "http://localhost:2391/DigestService.svc/HelloWorld";

        [TestMethod]
        public void Given_credentials_are_invalid_then_should_return_401()
        {
            var session = new DigestClientSession(ServiceUrl, Constants.Username, Constants.BadPassword);
            try
            {
                using (session.ExecuteRequest()) { }
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
            var session = new DigestClientSession(ServiceUrl, Constants.Username, Constants.Password);

            using (session.ExecuteRequest()) { }
        }

        [TestMethod]
        public void Given_message_is_replayed_then_should_return_401()
        {
            var session = new DigestClientSession(ServiceUrl, Constants.Username, Constants.Password);

            var nonce = NonceGenerator.Generate();

            session.ClientNonceGeneratorFunc = () => { return nonce; };

            using (session.ExecuteRequest()) { }
            try
            {
                using (session.ExecuteRequest()) { }
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }

        [TestMethod]
        public void Given_server_nonce_is_not_valid_then_should_return_401()
        {
            var session = new DigestClientSession(ServiceUrl, Constants.Username, Constants.BadPassword);
            try
            {
                //Overwrite server nonce
                session.ServerNonce = NonceGenerator.Generate();
                using (session.ExecuteRequest()) { }
            }
            catch (WebException ex)
            {
                Assert.AreEqual(HttpStatusCode.Unauthorized, ((HttpWebResponse)ex.Response).StatusCode);
                return;
            }
            Assert.Fail("It shouldn't have succeeded");
        }

        [TestMethod]
        public void Given_server_opaque_is_not_valid_then_should_return_401()
        {
            var session = new DigestClientSession(ServiceUrl, Constants.Username, Constants.BadPassword);
            try
            {
                //Overwrite service opaque
                session.Opaque = NonceGenerator.Generate();
                using (session.ExecuteRequest()) { }
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
