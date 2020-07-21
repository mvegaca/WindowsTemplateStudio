using System;
using System.Windows;
using ControlzEx.Theming;
using Param_RootNamespace.Contracts.Services;
using Param_RootNamespace.Models;

namespace Param_RootNamespace.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        private bool IsHighContrastActive
                        => SystemParameters.HighContrast;

        public ThemeSelectorService()
        {
            ThemeManager.Current.ThemeChanged += OnThemeChanged;
        }

        public void InitializeTheme()
        {
            if (IsHighContrastActive)
            {
                SetHighContrastTheme();
                return;
            }

            var theme = GetCurrentTheme();
            SetTheme(theme);
        }

        public void SetTheme(AppTheme theme)
        {
            if (theme == AppTheme.Default)
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                ThemeManager.Current.SyncTheme();
            }
            else
            {
                ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithHighContrast;
                ThemeManager.Current.ChangeTheme(Application.Current, $"{theme}.Blue");
            }

            App.Current.Properties["Theme"] = theme.ToString();
        }

        public AppTheme GetCurrentTheme()
        {
            if (App.Current.Properties.Contains("Theme"))
            {
                var themeName = App.Current.Properties["Theme"].ToString();
                Enum.TryParse(themeName, out AppTheme theme);
                return theme;
            }

            return AppTheme.Default;
        }

        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (e.NewTheme.IsHighContrast)
            {
                SetHighContrastTheme();
            }
        }

        private void SetHighContrastTheme()
        {
            // TODO WTS: Set high contrast theme
            // You can add custom themes following the docs on https://mahapps.com/docs/themes/thememanager
        }
    }
}
