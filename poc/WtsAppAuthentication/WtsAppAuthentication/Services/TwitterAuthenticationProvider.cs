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
                string oauth_token = await GetTwitterRequestTokenAsync(_callbackURL, _consumerKey);
                string TwitterUrl = $"{_baseTwitterUrl}authorize?oauth_token={oauth_token}";
                Uri StartUri = new Uri(TwitterUrl);
                Uri EndUri = new Uri(_callbackURL);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, StartUri, EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    result.Success = true;
                    result.ResponseData = WebAuthenticationResult.ResponseData.ToString();
                    await GetTwitterUserNameAsync(WebAuthenticationResult.ResponseData.ToString());
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
            }
            return result;
        }

        private async Task<string> GetTwitterRequestTokenAsync(string twitterCallbackUrl, string consumerKey)
        {
            // Acquiring a request token
            string TwitterUrl = "https://api.twitter.com/oauth/request_token";

            string nonce = GetNonce();
            string timeStamp = GetTimeStamp();
            string SigBaseStringParams = "oauth_callback=" + Uri.EscapeDataString(twitterCallbackUrl);
            SigBaseStringParams += "&" + "oauth_consumer_key=" + consumerKey;
            SigBaseStringParams += "&" + "oauth_nonce=" + nonce;
            SigBaseStringParams += "&" + "oauth_signature_method=HMAC-SHA1";
            SigBaseStringParams += "&" + "oauth_timestamp=" + timeStamp;
            SigBaseStringParams += "&" + "oauth_version=1.0";
            string SigBaseString = "GET&";
            SigBaseString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(SigBaseStringParams);
            string Signature = GetSignature(SigBaseString, _consumerSecret);

            TwitterUrl += "?" + SigBaseStringParams + "&oauth_signature=" + Uri.EscapeDataString(Signature);
            HttpClient httpClient = new HttpClient();
            string GetResponse = await httpClient.GetStringAsync(new Uri(TwitterUrl));

            string request_token = null;
            string oauth_token_secret = null;
            string[] keyValPairs = GetResponse.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                string[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            return request_token;
        }

        private async Task GetTwitterUserNameAsync(string webAuthResultResponseData)
        {
            // Acquiring a access_token first
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf("oauth_token"));
            string request_token = null;
            string oauth_verifier = null;
            String[] keyValPairs = responseData.Split('&');

            for (int i = 0; i < keyValPairs.Length; i++)
            {
                String[] splits = keyValPairs[i].Split('=');
                switch (splits[0])
                {
                    case "oauth_token":
                        request_token = splits[1];
                        break;
                    case "oauth_verifier":
                        oauth_verifier = splits[1];
                        break;
                }
            }

            String TwitterUrl = "https://api.twitter.com/oauth/access_token";

            string timeStamp = GetTimeStamp();
            string nonce = GetNonce();

            string parameters = "oauth_consumer_key=" + _consumerKey;
            parameters += "&" + "oauth_nonce=" + nonce;
            parameters += "&" + "oauth_signature_method=HMAC-SHA1";
            parameters += "&" + "oauth_timestamp=" + timeStamp;
            parameters += "&" + "oauth_token=" + request_token;
            parameters += "&" + "oauth_version=1.0";
            string signatureString = "POST&";
            signatureString += Uri.EscapeDataString(TwitterUrl) + "&" + Uri.EscapeDataString(parameters);

            string signature = GetSignature(signatureString, _consumerSecret);

            HttpStringContent httpContent = new HttpStringContent("oauth_verifier=" + oauth_verifier, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            string authorizationHeaderParams = "oauth_consumer_key=\"" + _consumerKey + "\", oauth_nonce=\"" + nonce + "\", oauth_signature_method=\"HMAC-SHA1\", oauth_signature=\"" + Uri.EscapeDataString(signature) + "\", oauth_timestamp=\"" + timeStamp + "\", oauth_token=\"" + Uri.EscapeDataString(request_token) + "\", oauth_version=\"1.0\"";

            HttpClient httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", authorizationHeaderParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(TwitterUrl), httpContent);
            string response = await httpResponseMessage.Content.ReadAsStringAsync();

            string[] tokens = response.Split('&');
            string oauth_token_secret = null;
            string access_token = null;
            string screen_name = null;

            for (int i = 0; i < tokens.Length; i++)
            {
                String[] splits = tokens[i].Split('=');
                switch (splits[0])
                {
                    case "screen_name":
                        screen_name = splits[1];
                        break;
                    case "oauth_token":
                        access_token = splits[1];
                        break;
                    case "oauth_token_secret":
                        oauth_token_secret = splits[1];
                        break;
                }
            }

            if (access_token != null)
            {
                // Store access_token for futher use. See Scenario 5 (Account Management).
            }

            if (oauth_token_secret != null)
            {
                // Store oauth_token_secret for further use. See Scenario 5 (Account Management).
            }
            if (screen_name != null)
            {
                //rootPage.NotifyUser(screen_name + " is connected!", NotifyType.StatusMessage);
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

        string GetSignature(string sigBaseString, string consumerSecretKey)
        {
            IBuffer KeyMaterial = CryptographicBuffer.ConvertStringToBinary(consumerSecretKey + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider HmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey MacKey = HmacSha1Provider.CreateKey(KeyMaterial);
            IBuffer DataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer SignatureBuffer = CryptographicEngine.Sign(MacKey, DataToBeSigned);
            string Signature = CryptographicBuffer.EncodeToBase64String(SignatureBuffer);

            return Signature;
        }
    }
}
