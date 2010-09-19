using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WcfHttpAuth.Digest
{
    public class DigestSession
    {
        private int sequenceNumber;
        private readonly string url;
        private readonly string username;
        private readonly string password;
        private string realm;
        private string serverNonce;

        public DigestSession(string url, string username, string password)
            :this(WebRequest.Create(url), username, password)
        {
            
        }

        public DigestSession(WebRequest request, string username, string password)
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
                ClientNonce = NonceGenerator.Generate(),
                Nonce = serverNonce,
                Path = request.RequestUri.PathAndQuery,
                SequenceNumber = sequenceNumber.ToString(),
                Username = username
            };
            token.CalculateDigest(realm, username, password, request.Method);

            request.Headers[HttpRequestHeader.Authorization] = 
                String.Format(CultureInfo.InvariantCulture, "Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\", opaque=\"{5}\", qop=auth, nc={6}, cnonce=\"{7}\"",
                                                            token.Username, realm, token.Nonce, token.Path, token.Digest, NonceGenerator.Generate(), token.SequenceNumber, token.ClientNonce);
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

            serverNonce = match.Groups["v"].Value;

            match = new Regex("realm=\"(?<v>.*?)\"", RegexOptions.IgnoreCase).Match(authHeader);
            if (!match.Groups["v"].Success)
                throw new InvalidOperationException("During session negotiation, the server failed to provide a realm value");

            realm = match.Groups["v"].Value;

        }
    }
}
