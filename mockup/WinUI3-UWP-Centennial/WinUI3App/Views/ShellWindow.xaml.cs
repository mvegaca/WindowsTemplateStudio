using Microsoft.UI.Xaml;

using WinUI3App.Contracts.Views;
using WinUI3App.Helpers;
using WinUI3App.ViewModels;

namespace WinUI3App.Views
{
    public sealed partial class ShellWindow : Window, IShellWindow
    {
        public ShellViewModel ViewModel { get; }

        public ShellWindow(ShellViewModel viewModel)
        {
            Title = "AppDisplayName".GetLocalized();
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
        }
    }
}
