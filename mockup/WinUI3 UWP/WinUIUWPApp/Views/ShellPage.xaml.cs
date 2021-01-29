using Microsoft.UI.Xaml.Controls;
using WinUIUWPApp.ViewModels;

namespace WinUIUWPApp.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; }

        public ShellPage(ShellViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
        }
    }
}