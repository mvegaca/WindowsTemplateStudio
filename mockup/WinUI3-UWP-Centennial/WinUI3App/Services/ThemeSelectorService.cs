using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using Windows.Storage;
using Windows.UI.Core;

using WinUI3App.Contracts.Services;
using WinUI3App.Helpers;

namespace WinUI3App.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public ElementTheme Theme { get; set; } = ElementTheme.Default;

        public async Task InitializeAsync()
        {
            // Temporarily commenting out as workaround for issue https://github.com/microsoft/WindowsTemplateStudio/issues/3984
            // Theme = await LoadThemeFromSettingsAsync();
            await Task.CompletedTask;
        }

        public async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();

            // Temporarily commenting out as workaround for issue https://github.com/microsoft/WindowsTemplateStudio/issues/3984
            // await SaveThemeInSettingsAsync(Theme);
        }

        public async Task SetRequestedThemeAsync()
        {
#if CENTENNIAL
            InternalSetRequestedTheme();
            await Task.CompletedTask;
#else
            await App.MainWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                InternalSetRequestedTheme();
            });
#endif

#if !CENTENNIAL
#endif
        }

        private void InternalSetRequestedTheme()
        {
            if (App.MainWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme;
            }
        }

        private async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString());
        }
    }
}
