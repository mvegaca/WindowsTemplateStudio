using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using WinUI3App.Contracts.Services;

namespace WinUI3App.Activation
{
    public class BackgroundTaskActivationHandler : ActivationHandler<BackgroundActivatedEventArgs>
    {
        private readonly IBackgroundTaskService _backgroundTaskService;

        public BackgroundTaskActivationHandler(IBackgroundTaskService backgroundTaskService)
        {
            _backgroundTaskService = backgroundTaskService;
        }

        protected override async Task HandleInternalAsync(BackgroundActivatedEventArgs args)
        {
            _backgroundTaskService.Start(args.TaskInstance);
            await Task.CompletedTask;
        }
    }
}
