namespace Param_RootNamespace.Services
{
    public class ThemeSelectorService : IThemeSelectorService
    {

        public async Task SetRequestedThemeAsync()
        {
//{[{
            if (App.MainWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme;
            }

//}]}
        }
    }
}
