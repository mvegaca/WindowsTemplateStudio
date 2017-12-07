using System.Threading.Tasks;
using Windows.Storage;
using WtsAppAuthentication.Models;
using WtsAppAuthentication.Helpers;
using System;

namespace WtsAppAuthentication.Services
{
    public static class AuthenticationService
    {
        private const string _settingsKeyAuthentication = "SettingsKeyAuthentication";
        private static bool _isDataLoaded;

        public static AuthenticationModel Data { get; private set; }

        public static async Task InitializeAsync()
        {
            if (!_isDataLoaded)
            {
                Data = await ApplicationData.Current.LocalFolder.ReadAsync<AuthenticationModel>(_settingsKeyAuthentication);
                if (Data == null)
                {
                    Data = new AuthenticationModel();
                }
            }
        }

        internal static async Task SaveDataAsync()
        {
            await ApplicationData.Current.LocalFolder.SaveAsync(_settingsKeyAuthentication, Data);
        }
    }
}
