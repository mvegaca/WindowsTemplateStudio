using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;

using WinUI3App.Activation;
using WinUI3App.Contracts.Services;
using WinUI3App.Helpers;
using WinUI3App.Views;

namespace WinUI3App.Services
{
    public class ActivationService : IActivationService
    {
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly INavigationService _navigationService;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly IBackgroundTaskService _backgroundTaskService;

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService, IBackgroundTaskService backgroundTaskService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
            _themeSelectorService = themeSelectorService;
            _backgroundTaskService = backgroundTaskService;
        }

        public async Task ActivateAsync(object activationArgs)
        {
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();

                if (App.MainWindow.Content == null)
                {
                    App.MainWindow.Content = Ioc.Default.GetService<ShellPage>();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
            if (IsInteractive(activationArgs))
            {
                // Ensure the current window is active
                App.MainWindow.Activate();

                // Tasks after activation
                await StartupAsync();
            }
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
            var activationHandler = _activationHandlers
                                                .FirstOrDefault(h => h.CanHandle(activationArgs));
            if (activationHandler != null)
            {
                await activationHandler.HandleAsync(activationArgs);
            }

            if (IsInteractive(activationArgs) && _defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
        }

        private async Task InitializeAsync()
        {
#if !CENTENNIAL
            await _backgroundTaskService.RegisterBackgroundTasksAsync().ConfigureAwait(false);
#endif
            await _themeSelectorService.InitializeAsync().ConfigureAwait(false);            
            await Task.CompletedTask;
        }

        private async Task StartupAsync()
        {
            await _themeSelectorService.SetRequestedThemeAsync();
            await Task.CompletedTask;
        }

        private bool IsInteractive(object args)
        {
#if CENTENNIAL
            return true;
#else
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                return launchArgs.UWPLaunchActivatedEventArgs is Windows.ApplicationModel.Activation.IActivatedEventArgs;
            }

            return false;
#endif
        }
    }
}
