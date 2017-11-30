using System;

using Windows.UI.Xaml.Controls;

using WtsAppAuthentication.ViewModels;

namespace WtsAppAuthentication.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame);
        }
    }
}
