using System;

using AppExtensionHost.ViewModels;

using Windows.UI.Xaml.Controls;

namespace AppExtensionHost.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
