using System;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.LoadData();
        }
    }
}
