using System;

using Windows.UI.Xaml.Controls;

using WtsAppAuthentication.ViewModels;

namespace WtsAppAuthentication.Views
{
    public sealed partial class AuthenticationPage : Page
    {
        public AuthenticationViewModel ViewModel { get; } = new AuthenticationViewModel();

        public AuthenticationPage()
        {
            InitializeComponent();
        }
    }
}
