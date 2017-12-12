using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using WtsAppAuthentication.Extensions;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class FacebookAuthenticationProvider : IAuthenticationProvider
    {
        private const string _baseApiUrl = "https://www.facebook.com/v2.11/dialog/oauth?";
        private const string _apiDialogOauthDisplay = "popup";
        private const string _apiDialogOauthResponseType = "token";

        private const string _paramClientId = "client_id";
        private const string _paramDisplay = "display";
        private const string _paramResponseType = "response_type";
        private const string _paramRedirectUri = "redirect_uri";
        private const string _paramAccessToken = "access_token";

        private string _clientID;

        public FacebookAuthenticationProvider(string clientID)
        {
            _clientID = clientID;
        }

        public Task<AuthenticationResult> AuthenticateAsync()
        {
            return AuthenticateAsync(null);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(Action privacyPolicyInvokedAction)
        {
            var result = new AuthenticationResult();
            try
            {
                var redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
                var parameters = new Dictionary<string, string>();
                parameters.Add(_paramClientId, _clientID);
                parameters.Add(_paramDisplay, _apiDialogOauthDisplay);
                parameters.Add(_paramResponseType, _apiDialogOauthResponseType);
                parameters.Add(_paramRedirectUri, redirectUri);

                var facebookURL = $"{_baseApiUrl}{AuthenticationHelper.GetRequestParameters(parameters)}";

                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(facebookURL));
                switch (webAuthenticationResult.ResponseStatus)
                {
                    case WebAuthenticationStatus.Success:
                        result.Success = true;
                        var data = webAuthenticationResult.ResponseData.ToString();
                        data = data.Substring(data.IndexOf(_paramAccessToken));
                        result.ResponseData = data.ReadParameters('&', '=');
                        break;                    
                    case WebAuthenticationStatus.UserCancel:
                        result.Reason = ReasonType.UserCancel;
                        break;
                    case WebAuthenticationStatus.ErrorHttp:
                        result.Reason = ReasonType.ErrorHttp;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                result.Success = false;
            }

            return result;
        }
    }
}
