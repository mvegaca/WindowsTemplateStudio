using System;

using AppExtension.ViewModels;

using Windows.UI.Xaml.Controls;

namespace AppExtension.Views
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
