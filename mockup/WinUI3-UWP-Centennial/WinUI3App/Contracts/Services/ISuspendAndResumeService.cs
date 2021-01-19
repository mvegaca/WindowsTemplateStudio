using System;
using System.Threading.Tasks;
using WinUI3App.Helpers;

namespace WinUI3App.Contracts.Services
{
    public interface ISuspendAndResumeService
    {
        event EventHandler<SuspendAndResumeArgs> OnBackgroundEntering;

        event EventHandler<SuspendAndResumeArgs> OnDataRestored;

        event EventHandler OnResuming;

        Task<bool> SaveStateAsync();

        void ResumeApp();

        Task RestoreSuspendAndResumeData();

        Task<SuspendAndResumeArgs> GetSuspendAndResumeData();
    }
}
