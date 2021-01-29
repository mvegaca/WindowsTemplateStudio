using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;

using WinUIUWPApp.Activation;
using WinUIUWPApp.Contracts.Services;
using WinUIUWPApp.Core.Contracts.Services;
using WinUIUWPApp.Core.Services;
using WinUIUWPApp.Services;
using WinUIUWPApp.ViewModels;
using WinUIUWPApp.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIUWPApp
{
    sealed partial class App : Application
    {
        public static Window MainWindow => Window.Current;

        public App()
        {
            this.InitializeComponent();
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(e);
        }

        protected override async void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            base.OnActivated(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }

        private System.IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IWebViewService, WebViewService>();

            // Core Services
            services.AddSingleton<ISampleDataService, SampleDataService>();

            // Views and ViewModels
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<WebViewViewModel>();
            services.AddTransient<WebViewPage>();
            services.AddTransient<FormViewModel>();
            services.AddTransient<FormPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            return services.BuildServiceProvider();
        }
    }
}
