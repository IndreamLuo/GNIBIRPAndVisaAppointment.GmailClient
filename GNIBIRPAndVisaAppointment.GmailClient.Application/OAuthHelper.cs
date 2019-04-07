using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GNIBIRPAndVisaAppointment.GmailClient.Application
{
    public class OAuthHelper
    {
        IConfiguration Configuration;

        public OAuthHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            SecurityToken = configuration["SecurityToken"];
            ClientId = configuration["ClientId"];
            ClientSecret = configuration["ClientSecret"];
            AuthorizedURL = configuration["RootUrl"];
            
            if (!AuthorizedURL.EndsWith("/"))
            {
                AuthorizedURL += "/";
            }

            AuthorizedURL += "authorization/authorized";
        }

        public string Code { get; private set; }
        public string Scope { get; private set; }

        public bool IsAuthorized { get; private set;}
        readonly string SecurityToken;
        readonly string ClientId;
        readonly string ClientSecret;
        readonly string AuthorizedURL;

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime Expiration { get; private set; }
        public string TokenType { get; private set; }

        public string OAuthURL(string requestingUrl) => string.Format("https://accounts.google.com/o/oauth2/v2/auth?{0}",
            string.Join("&", 
                "scope=https%3A%2F%2Fmail.google.com%2F",
                "access_type=offline",
                $"state={SecurityToken}%3D138r5719ru3e1%26url%3D{requestingUrl}",
                $"redirect_uri={AuthorizedURL}",
                "response_type=code",
                $"client_id={ClientId}",
                "prompt=consent",
                "include_granted_scopes=true"));

        static string TokenURL = "https://www.googleapis.com/oauth2/v4/token";

        public async Task LogAuthorizationAsync(string code, string scope)
        {
            Code = code;
            Scope = scope;

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(TokenURL, new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", AuthorizedURL),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                }));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic result = JObject.Parse(responseContent);
                    AccessToken = result.access_token;
                    RefreshToken = result.refresh_token;
                    Expiration = DateTime.Now.AddSeconds(Convert.ToInt32(result.expires_in));
                    TokenType = result.token_type;
                }
            }
        }

        public async Task RefreshAuthorizationAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(TokenURL, new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("refresh_token", RefreshToken),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("redirect_uri", AuthorizedURL),
                    new KeyValuePair<string, string>("grant_type", "refresh_token")
                }));

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    dynamic result = JObject.Parse(responseContent);
                    AccessToken = result.access_token;
                    Expiration = DateTime.Now.AddSeconds(Convert.ToInt32(result.expires_in));
                    TokenType = result.token_type;
                }
            }
        }

        public string GetRequestUrlFromState(string state)
        {
            var regex = new Regex("(?<SecurityToken>.*)=138r5719ru3e1&url=(?<requestingUrl>.*)");
            var regexResult = regex.Match(state);
            var requestingUrl = regexResult.Groups["requestingUrl"].Value;
            
            return requestingUrl;
        }

        public async Task<GoogleCredential> GetCredentialAsync()
        {
            if (AccessToken != null)
            {
                if (Expiration <= DateTime.Now)
                {
                    await RefreshAuthorizationAsync();
                }

                if (Expiration > DateTime.Now)
                {
                    return GoogleCredential.FromAccessToken(AccessToken);
                }
            }

            throw new GmailUnauthorizedException();
        }
    }
}