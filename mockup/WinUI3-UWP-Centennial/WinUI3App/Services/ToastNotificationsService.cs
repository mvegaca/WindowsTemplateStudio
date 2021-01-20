using System;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using WinUI3App.Contracts.Services;

namespace WinUI3App.Services
{
    public partial class ToastNotificationsService : IToastNotificationsService
    {
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            try
            {
#if Win32
                ToastNotificationManagerCompat.CreateToastNotifier().Show(toastNotification);
#else
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
#endif               
            }
            catch (Exception ex)
            {
                // TODO WTS: Adding ToastNotification can fail in rare conditions, please handle exceptions as appropriate to your scenario.
            }
        }
    }
}
