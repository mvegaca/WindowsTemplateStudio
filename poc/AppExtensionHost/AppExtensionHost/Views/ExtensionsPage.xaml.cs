using System;

using AppExtensionHost.ViewModels;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AppExtensionHost.Views
{
    public sealed partial class ExtensionsPage : Page
    {
        public ExtensionsViewModel ViewModel { get; } = new ExtensionsViewModel();

        public ExtensionsPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.InitializeAsync();
        }
    }
}
