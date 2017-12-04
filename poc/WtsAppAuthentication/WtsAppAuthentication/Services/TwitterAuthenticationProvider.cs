using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class TwitterAuthenticationProvider : IAuthenticationProvider
    {
        private const string _baseTwitterUrl = "https://api.twitter.com/oauth/";
        private string _consumerKey;
        private string _consumerSecret;
        private string _callbackURL;

        public TwitterAuthenticationProvider(string consumerKey, string consumerSecret, string callbackURL)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _callbackURL = callbackURL;
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {

            var result = new AuthenticationResult();
            try
            {
                var oauthToken = await GetTwitterRequestTokenAsync();
                var twitterUrl = $"{_baseTwitterUrl}authorize?oauth_token={oauthToken}";
                var startUri = new Uri(twitterUrl);
                var endUri = new Uri(_callbackURL);

                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri, endUri);
                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    result.Success = true;
                    result.ResponseData = webAuthenticationResult.ResponseData.ToString();
                    await GetTwitterUserNameAsync(webAuthenticationResult.ResponseData.ToString());
                }
                else if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                }
                else
                {
                }
            }
            catch (Exception)
            {
                result.Success = false;
            }
            return result;
        }

        private async Task<string> GetTwitterRequestTokenAsync()
        {
            var twitterUrl = $"{_baseTwitterUrl}request_token";
            var nonce = GetNonce();
            var timeStamp = GetTimeStamp();
            var parameters = "oauth_callback=" + Uri.EscapeDataString(_callbackURL);
            parameters += "&" + "oauth_consumer_key=" + _consumerKey;
            parameters += "&" + "oauth_nonce=" + nonce;
            parameters += "&" + "oauth_signature_method=HMAC-SHA1";
            parameters += "&" + "oauth_timestamp=" + timeStamp;
            parameters += "&" + "oauth_version=1.0";
            var signatureString = "GET&";
            signatureString += Uri.EscapeDataString(twitterUrl) + "&" + Uri.EscapeDataString(parameters);
            var signature = GetSignature(signatureString);

            twitterUrl += "?" + parameters + "&oauth_signature=" + Uri.EscapeDataString(signature);
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(new Uri(twitterUrl));

            string requestToken = null;
            string oauthTokenSecret = null;
            string[] keyValPairs = response.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        requestToken = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauthTokenSecret = splits[1];
                        break;
                }
            }

            return requestToken;
        }

        private async Task GetTwitterUserNameAsync(string webAuthResultResponseData)
        {
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"));
            string requestToken = null;
            string oauthVerifier = null;
            var keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                var splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        requestToken = splits[1];
                        break;
                    case "oauth_verifier":
                        oauthVerifier = splits[1];
                        break;
                }
            }

            var twitterUrl = $"{_baseTwitterUrl}access_token";

            var timeStamp = GetTimeStamp();
            var nonce = GetNonce();

            var parameters = "oauth_consumer_key=" + _consumerKey;
            parameters += "&" + "oauth_nonce=" + nonce;
            parameters += "&" + "oauth_signature_method=HMAC-SHA1";
            parameters += "&" + "oauth_timestamp=" + timeStamp;
            parameters += "&" + "oauth_token=" + requestToken;
            parameters += "&" + "oauth_version=1.0";
            var signatureString = "POST&";
            signatureString += Uri.EscapeDataString(twitterUrl) + "&" + Uri.EscapeDataString(parameters);

            var signature = GetSignature(signatureString);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + oauthVerifier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            var authorizationHeaderParams = "oauth_consumer_key=\"" + _consumerKey + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(signature) + "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" + Uri.EscapeDataString(requestToken) + "\", oauth_version=\"1.0\"";

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(twitterUrl), httpContent);
            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            var tokens = response.Split('&');
            string oauthTokenSecret = null;
            string accessToken = null;
            string screenName = null;

            for (int i = 0; i < tokens.Length; i++)
            {
                var splits = tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screenName = splits[1];
                        break;
                    case "oauth_token":
                        accessToken = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauthTokenSecret = splits[1];
                        break;
                }
            }

            if (accessToken != null)
            {
            }
            if (oauthTokenSecret != null)
            {
            }
            if (screenName != null)
            {
            }
        }

        string GetNonce()
        {
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        string GetTimeStamp()
        {
            TimeSpan SinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(SinceEpoch.TotalSeconds).ToString();
        }

        string GetSignature(string sigBaseString)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(_consumerSecret + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}
