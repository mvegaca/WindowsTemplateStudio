using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using WinUI3App.BackgroundTasks;

namespace WinUI3App.Contracts.Services
{
    public interface IBackgroundTaskService
    {
        Task RegisterBackgroundTasksAsync();

        BackgroundTaskRegistration GetBackgroundTasksRegistration<T>() where T : BackgroundTask;

        void Start(IBackgroundTaskInstance taskInstance);
    }
}
