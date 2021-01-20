using Windows.UI.Notifications;

namespace WinUI3App.Contracts.Services
{
    public interface IToastNotificationsService
    {
        void ShowToastNotification(ToastNotification toastNotification);

        void ShowToastNotificationSample();
    }
}
