using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.System;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;
using WinRT;
using WinUI3App.Activation;
using WinUI3App.Contracts.Services;
using WinUI3App.Helpers;
using WinUI3App.Services;
using WinUI3App.ViewModels;
using WinUI3App.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace WinUI3App
{
    public partial class App : Application
    {
#if Win32
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
        internal interface IWindowNative { System.IntPtr WindowHandle { get; } }

        public System.IntPtr WindowHandle { get; private set; }

        public static readonly Window MainWindow = new Window() { Title = "AppDisplayName".GetLocalized() };
#else
        public static Window MainWindow => Window.Current;
#endif

        public App()
        {
            InitializeComponent();
#if UWP
            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;
            Suspending += OnSuspending;
#endif
            UnhandledException += App_UnhandledException;
            Ioc.Default.ConfigureServices(ConfigureServices());
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
#if Win32
            // https://docs.microsoft.com/windows/uwp/design/shell/tiles-and-notifications/send-local-toast
            ToastNotificationManagerCompat.OnActivated += async (toastArgs) =>
            {
                // This fails because we should handle thread sync using a dispatcher
                await activationService.ActivateAsync(toastArgs.Argument);
            };

            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                // ToastNotificationActivator code will run after this completes and will show a window if necessary.
                return;
            }
#endif

            await activationService.ActivateAsync(args);
#if Win32
            WindowHandle = MainWindow.As<IWindowNative>().WindowHandle;
#endif
        }

        protected override async void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            base.OnActivated(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }

        protected override async void OnBackgroundActivated(Windows.ApplicationModel.Activation.BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
            var activationService = Ioc.Default.GetService<IActivationService>();
            await activationService.ActivateAsync(args);
        }

        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
        }

        private System.IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();
            services.AddTransient<IActivationHandler, ToastNotificationsActivationHandler>();
#if UWP
            services.AddTransient<IActivationHandler, BackgroundTaskActivationHandler>();
            services.AddTransient<IActivationHandler, SuspendAndResumeActivationHandler>();            
#endif

            // Other Activation Handlers

            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IToastNotificationsService, ToastNotificationsService>();
#if UWP
            services.AddSingleton<IBackgroundTaskService, BackgroundTaskService>();
            services.AddSingleton<ISuspendAndResumeService, SuspendAndResumeService>();            
#endif

            // Core Services

            // Views and ViewModels
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            return services.BuildServiceProvider();
        }

#if UWP
        private async void App_EnteredBackground(object sender, Windows.ApplicationModel.EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            var suspendAndResumeService = Ioc.Default.GetService<ISuspendAndResumeService>();
            await suspendAndResumeService.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            var suspendAndResumeService = Ioc.Default.GetService<ISuspendAndResumeService>();
            suspendAndResumeService.ResumeApp();
        }
#endif
    }
}
