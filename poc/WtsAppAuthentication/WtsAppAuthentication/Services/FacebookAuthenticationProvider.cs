using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class FacebookAuthenticationProvider : IAuthenticationProvider
    {
        private const string _baseFacebookUrl = "https://www.facebook.com/v2.11/dialog/oauth?";
        private string _clientID;

        public FacebookAuthenticationProvider(string clientID)
        {
            _clientID = clientID;
        }

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            var result = new AuthenticationResult();
            try
            {
                var redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();
                var facebookURL = $"{_baseFacebookUrl}client_id={_clientID}&display=popup&response_type=token&redirect_uri={redirectUri}";
                var webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(facebookURL));
                switch (webAuthenticationResult.ResponseStatus)
                {
                    case WebAuthenticationStatus.Success:
                        result.Success = true;
                        result.ResponseData = webAuthenticationResult.ResponseData.ToString();
                        break;
                    case WebAuthenticationStatus.ErrorHttp:
                        result.ResponseData = webAuthenticationResult.ResponseErrorDetail.ToString();
                        break;
                    case WebAuthenticationStatus.UserCancel:
                        break;
                    default:
                        result.ResponseData = webAuthenticationResult.ResponseData.ToString();
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
