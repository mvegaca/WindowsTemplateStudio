using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using WtsAppAuthentication.Extensions;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class TwitterAuthenticationProvider : IAuthenticationProvider
    {
        private const string _baseTwitterUrl = "https://api.twitter.com/oauth/";
        private const string _apiServiceRequestToken = "request_token";
        private const string _apiServiceAuthorize = "authorize";
        private const string _apiServiceAccessToken = "access_token";
        private const string _apiSignatureMethodSHA = "HMAC-SHA1";
        private const string _apiOauthVersion = "1.0";
        private const string _signatureMacAlgorithm = "HMAC_SHA1";
        private const string _requestHeadersContentType = "application/x-www-form-urlencoded";

        private const string _paramOauthToken = "oauth_token";
        private const string _paramOauthTokenSecret = "oauth_token_secret";
        private const string _paramOauthVerifier = "oauth_verifier";
        private const string _paramScreenName = "screen_name";
        private const string _paramOauthCallback = "oauth_callback";
        private const string _paramOauthNonce = "oauth_nonce";
        private const string _paramOauthConsumerKey = "oauth_consumer_key";
        private const string _paramOauthSignatureMethod = "oauth_signature_method";
        private const string _paramOauthTimeStamp = "oauth_timestamp";
        private const string _paramOauthVersion = "oauth_version";
        private const string _paramOauthSignature = "oauth_signature";

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
                var requestToken = await RequestTokenAsync();
                var twitterUrl = $"{_baseTwitterUrl}{_apiServiceAuthorize}?{_paramOauthToken}={requestToken[_paramOauthToken]}";
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

        private async Task<Dictionary<string, string>> RequestTokenAsync()
        {
            var twitterUrl = $"{_baseTwitterUrl}{_apiServiceRequestToken}";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add(_paramOauthCallback, Uri.EscapeDataString(_callbackURL));
            parameters.Add(_paramOauthConsumerKey, _consumerKey);
            parameters.Add(_paramOauthNonce, GetNonce());
            parameters.Add(_paramOauthSignatureMethod, _apiSignatureMethodSHA);
            parameters.Add(_paramOauthTimeStamp, GetTimeStamp());
            parameters.Add(_paramOauthVersion, _apiOauthVersion);            

            string requestParameters = GetRequestParameters(parameters);

            var signatureString = $"GET&{Uri.EscapeDataString(twitterUrl)}&{Uri.EscapeDataString(requestParameters)}";
            var signature = GetSignature(signatureString);
            twitterUrl += $"?{requestParameters}&{_paramOauthSignature}={ Uri.EscapeDataString(signature)}";
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(new Uri(twitterUrl));

            return response.ReadParameters('&', '=');
        }

        private async Task<Dictionary<string, string>> GetTwitterUserNameAsync(string webAuthResultResponseData)
        {
            string responseData = webAuthResultResponseData.Substring(webAuthResultResponseData.IndexOf(_paramOauthToken));
            var readedParams = responseData.ReadParameters('&', '=');
            var twitterUrl = $"{_baseTwitterUrl}{_apiServiceAccessToken}";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            var nonce = GetNonce();
            var timeStamp = GetTimeStamp();
            parameters.Add(_paramOauthCallback, _consumerKey);
            parameters.Add(_paramOauthNonce, nonce);
            parameters.Add(_paramOauthSignatureMethod, _apiSignatureMethodSHA);
            parameters.Add(_paramOauthTimeStamp, timeStamp);
            parameters.Add(_paramOauthToken, readedParams[_paramOauthToken]);
            parameters.Add(_paramOauthVersion, _apiOauthVersion);

            string requestParameters = GetRequestParameters(parameters);
            var signatureString = "POST&";
            signatureString += Uri.EscapeDataString(twitterUrl) + "&" + Uri.EscapeDataString(requestParameters);

            var signature = GetSignature(signatureString);

            HttpStringContent httpContent = new HttpStringContent($"{_paramOauthVerifier}={readedParams[_paramOauthVerifier]}", UnicodeEncoding.Utf8);
            httpContent.Headers.ContentType = HttpMediaTypeHeaderValue.Parse(_requestHeadersContentType);
            var headerParams = $"{_paramOauthCallback}=\"{_consumerKey}\", {_paramOauthNonce}=\"{nonce}\", {_paramOauthSignatureMethod}=\"{_apiSignatureMethodSHA}\", {_paramOauthSignature}=\"{Uri.EscapeDataString(signature)}\", {_paramOauthTimeStamp}=\"{timeStamp}\", {_paramOauthToken}=\"{Uri.EscapeDataString(readedParams[_paramOauthToken])}\", {_paramOauthVersion}=\"{_apiOauthVersion}\"";

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("OAuth", headerParams);
            var httpResponseMessage = await httpClient.PostAsync(new Uri(twitterUrl), httpContent);
            var response = await httpResponseMessage.Content.ReadAsStringAsync();
            return response.ReadParameters('&', '=');
        }

        private string GetNonce()
        {
            Random rand = new Random();
            int nonce = rand.Next(1000000000);
            return nonce.ToString();
        }

        private string GetTimeStamp()
        {
            TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return Math.Round(sinceEpoch.TotalSeconds).ToString();
        }

        private string GetSignature(string sigBaseString)
        {
            IBuffer keyMaterial = CryptographicBuffer.ConvertStringToBinary(_consumerSecret + "&", BinaryStringEncoding.Utf8);
            MacAlgorithmProvider hmacSha1Provider = MacAlgorithmProvider.OpenAlgorithm(_signatureMacAlgorithm);
            CryptographicKey macKey = hmacSha1Provider.CreateKey(keyMaterial);
            IBuffer dataToBeSigned = CryptographicBuffer.ConvertStringToBinary(sigBaseString, BinaryStringEncoding.Utf8);
            IBuffer signatureBuffer = CryptographicEngine.Sign(macKey, dataToBeSigned);
            return CryptographicBuffer.EncodeToBase64String(signatureBuffer);
        }

        private string GetRequestParameters(Dictionary<string, string> parameters)
        {
            var requestParameters = string.Empty;
            foreach (var param in parameters)
            {
                if (string.IsNullOrEmpty(param.Key))
                {
                    throw new ArgumentNullException("parameterKey");
                }
                if (string.IsNullOrEmpty(requestParameters))
                {
                    requestParameters = $"{param.Key}={param.Value}";
                }
                else
                {
                    requestParameters += $"&{param.Key}={param.Value}";
                }
            }
            return requestParameters;
        }
    }
}
