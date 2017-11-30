using System;

using Windows.UI.Xaml.Controls;

using WtsAppAuthentication.ViewModels;

namespace WtsAppAuthentication.Views
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
