using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WcfRestAuth.Digest
{
    public class DigestClientSession
    {
        private int sequenceNumber;
        private readonly string url;
        private readonly string username;
        private readonly string password;
        private string realm;

        internal string ServerNonce { get; set; }
        internal string Opaque { get; set; }

        //To be used in tests only
        private Func<string> clientNonceGeneratorFunc = () => NonceGenerator.Generate();
        internal Func<string> ClientNonceGeneratorFunc
        {
            get { return clientNonceGeneratorFunc; }
            set { clientNonceGeneratorFunc = value; }
        }

        public DigestClientSession(string url, string username, string password)
            :this(WebRequest.Create(url), username, password)
        {
            
        }

        public DigestClientSession(WebRequest request, string username, string password)
        {
            this.username = username;
            this.password = password;
            this.url = request.RequestUri.ToString();
            try
            {
                WebRequest.Create(url).GetResponse();
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode != HttpStatusCode.Unauthorized)
                    throw;
                ExtractChallengeFields(ex.Response);
            }
        }
       
        public WebResponse ExecuteRequest()
        {
            return ExecuteRequest(WebRequest.Create(url));
        }

        public WebResponse ExecuteRequest(WebRequest request)
        {
            sequenceNumber++;
            var token = new DigestToken
            {
                ClientNonce = ClientNonceGeneratorFunc.Invoke(),
                ServerNonce = ServerNonce,
                Path = request.RequestUri.PathAndQuery,
                SequenceNumber = sequenceNumber.ToString(),
                Username = username
            };
            token.CalculateDigest(realm, username, password, request.Method);

            request.Headers[HttpRequestHeader.Authorization] = 
                String.Format(CultureInfo.InvariantCulture, "Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\", opaque=\"{5}\", qop=auth, nc={6}, cnonce=\"{7}\"",
                                                            token.Username, realm, token.ServerNonce, token.Path, token.Digest, Opaque, token.SequenceNumber, token.ClientNonce);
            return request.GetResponse();
        }

        private void ExtractChallengeFields(WebResponse response)
        {
            var authHeader = response.Headers[HttpResponseHeader.WwwAuthenticate];
            response.Close();

            if (authHeader != null && authHeader.StartsWith("Digest"))
            {
                SetChallengeFieldsFromServer(authHeader);
            }
            else
            {
                throw new InvalidOperationException("The server does not require Digest auth");
            }
        }

        private void SetChallengeFieldsFromServer(string authHeader)
        {
            var match = new Regex("nonce=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(authHeader);
            if (!match.Groups["v"].Success)
                throw new InvalidOperationException("During session negotiation, the server failed to provide a nonce");

            ServerNonce = match.Groups["v"].Value;

            match = new Regex("realm=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(authHeader);
            if (!match.Groups["v"].Success)
                throw new InvalidOperationException("During session negotiation, the server failed to provide a realm value");
            realm = match.Groups["v"].Value;

            match = new Regex("opaque=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(authHeader);
            if (!match.Groups["v"].Success)
                throw new InvalidOperationException("During session negotiation, the server failed to provide a realm value");

            Opaque = match.Groups["v"].Value;

        }
    }
}
