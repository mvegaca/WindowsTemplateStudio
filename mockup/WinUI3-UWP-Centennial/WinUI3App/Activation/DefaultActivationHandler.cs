using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using WinUI3App.Contracts.Services;
using WinUI3App.ViewModels;

namespace WinUI3App.Activation
{
    public class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly INavigationService _navigationService;
        private readonly IToastNotificationsService _toastNotificationsService;

        public DefaultActivationHandler(INavigationService navigationService, IToastNotificationsService toastNotificationsService)
        {
            _navigationService = navigationService;
            _toastNotificationsService = toastNotificationsService;
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            _navigationService.NavigateTo(typeof(MainViewModel).FullName, args.Arguments);

            // TODO WTS: Remove or change this sample which shows a toast notification when the app is launched.
            // You can use this sample to create toast notifications where needed in your app.
            _toastNotificationsService.ShowToastNotificationSample();
            await Task.CompletedTask;
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // None of the ActivationHandlers has handled the app activation
            return _navigationService.Frame.Content == null;
        }
    }
}
