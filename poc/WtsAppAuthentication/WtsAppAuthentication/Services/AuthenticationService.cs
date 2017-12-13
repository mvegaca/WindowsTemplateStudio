using System.Threading.Tasks;
using Windows.Storage;
using WtsAppAuthentication.Models;
using WtsAppAuthentication.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Security.Credentials;

namespace WtsAppAuthentication.Services
{
    public static class AuthenticationService
    {
        private const string _credentialsResource = "MyApp";
        private const string _settingsKeyRememberCredentials = "SettingsKeyRememberCredentials";
        private const string _settingsKeyIsLoggedIn = "SettingsKeyIsLoggedIn";
        private const string _settingsKeyLastUserName = "SettingsKeyLastUserName";

        private static PasswordVault _passwordVault = new PasswordVault();
        private static IEnumerable<AuthenticationProviderBase> _authenticationProviders;
        private static bool _isDataLoaded;

        public static bool RememberCredentials { get; private set; }

        public static string LastUserName { get; private set; }

        public static bool IsLoggedIn { get; private set; }

        public static event EventHandler OnPrivacyPolicyInvoked;

        public static async Task InitializeAsync()
        {
            if (!_isDataLoaded)
            {
                _authenticationProviders = GetAuthenticationProviders();
                RememberCredentials = await ApplicationData.Current.LocalFolder.ReadAsync<bool>(_settingsKeyRememberCredentials);
                IsLoggedIn = await ApplicationData.Current.LocalFolder.ReadAsync<bool>(_settingsKeyIsLoggedIn);
                LastUserName = await ApplicationData.Current.LocalFolder.ReadAsync<string>(_settingsKeyLastUserName);
                _isDataLoaded = true;
            }
        }

        private static IEnumerable<AuthenticationProviderBase> GetAuthenticationProviders()
        {
            yield return new EmailAuthenticationProvider();
            yield return new MicrosoftAuthenticationProvider(InvokedOnPrivacyPolicy);
            yield return new GoogleAuthenticationProvider();
            // TODO WTS: Add your Facebook Client ID
            yield return new FacebookAuthenticationProvider("");
            // TODO WTS: Add your Twitter Consumer Key, Consumer Secret and CallBack URL
            yield return new TwitterAuthenticationProvider("", "", "https://github.com/Microsoft/WindowsTemplateStudio/");
        }

        private static void InvokedOnPrivacyPolicy()
        {
            OnPrivacyPolicyInvoked?.Invoke(null, EventArgs.Empty);
        }

        public static Task<AuthenticationResult> AuthenticateAsync(string providerId)
        {
            var provider = GetAuthenticationProvider(providerId);
            return provider.AuthenticateAsync();
        }

        public static void ConfigureProviderParameter(string providerId, string parameterName, string parameterValue)
        {
            var provider = GetAuthenticationProvider(providerId);
            provider.ConfigureParameter(parameterName, parameterValue);
        }

        public static string RetrievePassword(string userName)
        {
            var results = _passwordVault.RetrieveAll();
            if (results != null)
            {
                var credentials = results.FirstOrDefault(c => c.UserName == userName);
                if (credentials != null)
                {
                    credentials.RetrievePassword();
                    return credentials.Password;
                }
            }
            return string.Empty;
        }

        public static void SaveCredentials(string userName, string password)
        {
            _passwordVault.Add(new PasswordCredential(_credentialsResource, userName, password));
        }

        public static void DeleteCredentials(string userName, string password)
        {
            _passwordVault.Remove(new PasswordCredential(_credentialsResource, userName, password));
        }

        private static AuthenticationProviderBase GetAuthenticationProvider(string providerId)
        {
            var provider = _authenticationProviders.FirstOrDefault(p => p.ProviderId == providerId);
            if (provider == null)
            {
                throw new Exception($"There is not configured an authentication provider for Id: {provider}");
            }
            return provider;
        }

        internal static async Task LogOutAsync()
        {
            IsLoggedIn = false;
            await ApplicationData.Current.LocalFolder.SaveAsync<bool>(_settingsKeyIsLoggedIn, false);
        }

        internal static async Task LogInAsync()
        {
            IsLoggedIn = true;
            await ApplicationData.Current.LocalFolder.SaveAsync<bool>(_settingsKeyIsLoggedIn, false);
        }

        internal static async Task SetRememberCredentialsAsync(bool rememberCredentials)
        {
            RememberCredentials = rememberCredentials;
            await ApplicationData.Current.LocalFolder.SaveAsync<bool>(_settingsKeyRememberCredentials, rememberCredentials);
        }

        internal static async Task SetLastUserNameAsync(string lastUserName)
        {
            LastUserName = lastUserName;
            await ApplicationData.Current.LocalFolder.SaveAsync<string>(_settingsKeyLastUserName, LastUserName);
        }
    }
}
