using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3App.Contracts.Services;

namespace WinUI3App.Activation
{
    public class SuspendAndResumeActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly INavigationService _navigationService;
        private readonly ISuspendAndResumeService _suspendAndResumeService;

        public SuspendAndResumeActivationHandler(INavigationService navigationService, ISuspendAndResumeService suspendAndResumeService)
        {
            _suspendAndResumeService = suspendAndResumeService;
            _navigationService = navigationService;
        }

        // This method restores application state when the App is launched after termination, it navigates to the stored Page passing the recovered state data.
        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
        {
            var saveState = await _suspendAndResumeService.GetSuspendAndResumeData();
            if (saveState?.Target != null && typeof(ObservableRecipient).IsAssignableFrom(saveState.Target))
            {
                _navigationService.NavigateTo(saveState.Target.FullName, saveState.SuspensionState);
            }
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs args)
        {
            // Application State must only be restored if the App was terminated during suspension.
            return args.UWPLaunchActivatedEventArgs.PreviousExecutionState == Windows.ApplicationModel.Activation.ApplicationExecutionState.Terminated;
        }
    }
}
