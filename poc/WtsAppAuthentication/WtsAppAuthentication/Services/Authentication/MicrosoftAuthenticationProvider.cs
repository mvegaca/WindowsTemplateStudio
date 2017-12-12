using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class MicrosoftAuthenticationProvider : IAuthenticationProvider
    {
        private const string _baseApiUrl = "https://apis.live.net/v5.0/me?";
        private const string _authenticationProviderId = "https://login.microsoft.com";
        private const string _accountAuthority = "consumers";
        private const string _accountScopeRequested = "wl.basic";
        private const string _paramAccessToken = "access_token";
        private const string _paramId = "id";
        private const string _paramName = "name";
        private const string _paramFirstName = "first_name";
        private const string _paramLastName = "last_name";
        private const string _paramLink = "link";
        private const string _paramGender = "gender";
        private const string _paramLocale = "locale";
        private const string _paramUpdatedTime = "updated_time";
        private const string _paramToken = "token";

        // TODO WTS: Read more information about using Web Account Manager
        // https://docs.microsoft.com/windows/uwp/security/web-account-manager
        private WebAccountProvider _provider;
        private TaskCompletionSource<AuthenticationResult> _tcs;
        private Action _privacyPolicyInvokedAction;

        public MicrosoftAuthenticationProvider()
        {
            _tcs = new TaskCompletionSource<AuthenticationResult>();
        }

        public Task<AuthenticationResult> AuthenticateAsync()
        {
            return AuthenticateAsync(null);
        }

        public Task<AuthenticationResult> AuthenticateAsync(Action privacyPolicyInvokedAction)
        {
            _privacyPolicyInvokedAction = privacyPolicyInvokedAction;
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;
            AccountsSettingsPane.Show();
            return _tcs.Task;
        }

        private async void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
                sender.AccountCommandsRequested -= BuildPaneAsync;
                // TODO WTS: Add here the header text for account request popup.
                // args.HeaderText = "";
                _provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(_authenticationProviderId, _accountAuthority);
                var command = new WebAccountProviderCommand(_provider, GetMsaTokenAsync);
                args.WebAccountProviderCommands.Add(command);
                if (_privacyPolicyInvokedAction != null)
                {
                    args.Commands.Add(new SettingsCommand("privacypolicy", "Privacy Policy", OnPrivacyPolicyClicked));
                }
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void OnPrivacyPolicyClicked(IUICommand command)
        {
            _privacyPolicyInvokedAction?.Invoke();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {

            var authenticationResult = new AuthenticationResult();
            try
            {
                var request = new WebTokenRequest(command.WebAccountProvider, _accountScopeRequested);
                var result = await WebAuthenticationCoreManager.RequestTokenAsync(request);

                if (result.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    authenticationResult.Success = true;
                    authenticationResult.ResponseData = new Dictionary<string, string>();
                    string token = result.ResponseData[0].Token;
                    authenticationResult.ResponseData.Add(_paramToken, token);

                    var restApi = new Uri($"{_baseApiUrl}{_paramAccessToken}={token}");

                    using (var client = new HttpClient())
                    {
                        var infoResult = await client.GetAsync(restApi);
                        string content = await infoResult.Content.ReadAsStringAsync();

                        var jsonObject = JsonObject.Parse(content);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramId);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramName);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramFirstName);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramLastName);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramLink);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramGender);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramLocale);
                        TryReadParameter(authenticationResult.ResponseData, jsonObject, _paramUpdatedTime);
                    }
                }
                else if(result.ResponseStatus == WebTokenRequestStatus.UserCancel)
                {
                    authenticationResult.Reason = ReasonType.UserCancel;
                    authenticationResult.ErrorMessage = result.ResponseError.ErrorMessage;
                }
                else if (result.ResponseStatus == WebTokenRequestStatus.ProviderError)
                {
                    authenticationResult.Reason = ReasonType.ErrorHttp;
                    authenticationResult.ErrorMessage = result.ResponseError.ErrorMessage;
                }
                else
                {
                    authenticationResult.Reason = ReasonType.Unexpected;
                    authenticationResult.ErrorMessage = result.ResponseError.ErrorMessage;
                }
            }
            catch (Exception)
            {
                authenticationResult.Success = false;
                authenticationResult.Reason = ReasonType.Unexpected;
            }
            finally
            {
                _tcs.SetResult(authenticationResult);
            }
        }

        private void TryReadParameter(Dictionary<string, string> parameters, JsonObject jsonObject, string param)
        {
            if (jsonObject != null && jsonObject.ContainsKey(param))
            {
                parameters.Add(param, jsonObject[param].Stringify());
            }
        }
    }
}
