using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3App.Activation;
using WinUI3App.Contracts.Services;
using WinUI3App.Helpers;
using WinUI3App.Views;

namespace WinUI3App.Services
{
    public class ActivationService : IActivationService
    {
        private UIElement _shell = null;
        private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
        private readonly IEnumerable<IActivationHandler> _activationHandlers;
        private readonly INavigationService _navigationService;
        private readonly IThemeSelectorService _themeSelectorService;
#if !CENTENNIAL
        private readonly IBackgroundTaskService _backgroundTaskService;
        private readonly ISuspendAndResumeService _suspendAndResumeService;
#endif

        public ActivationService(ActivationHandler<LaunchActivatedEventArgs> defaultHandler, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService)
        {
            _defaultHandler = defaultHandler;
            _activationHandlers = activationHandlers;
            _navigationService = navigationService;
            _themeSelectorService = themeSelectorService;
#if !CENTENNIAL
            _backgroundTaskService = Ioc.Default.GetService<IBackgroundTaskService>();
            _suspendAndResumeService = Ioc.Default.GetService<ISuspendAndResumeService>();
#endif
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
                    _shell = Ioc.Default.GetService<ShellPage>();
                    App.MainWindow.Content = _shell ?? new Frame();
                }
            }

            // Depending on activationArgs one of ActivationHandlers or DefaultActivationHandler
            // will navigate to the first page
            await HandleActivationAsync(activationArgs);
            if (IsInteractive(activationArgs))
            {
#if !CENTENNIAL
                var launchActivation = activationArgs as LaunchActivatedEventArgs;
                var activation = launchActivation.UWPLaunchActivatedEventArgs as Windows.ApplicationModel.Activation.IActivatedEventArgs;
                if (activation.PreviousExecutionState == Windows.ApplicationModel.Activation.ApplicationExecutionState.Terminated)
                {
                    await _suspendAndResumeService.RestoreSuspendAndResumeData();
                }
#endif

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
