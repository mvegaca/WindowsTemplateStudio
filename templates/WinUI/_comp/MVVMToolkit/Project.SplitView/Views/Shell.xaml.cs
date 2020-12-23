using Param_RootNamespace.ViewModels;

namespace Param_RootNamespace.Views
{
    public sealed partial class Shell : SHELL_TYPE
    {
        public ShellViewModel ViewModel { get; }

        public Shell(ShellViewModel viewModel)
        {
            Title = "AppDisplayName".GetLocalized();
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.NavigationService.Frame = shellFrame;
            ViewModel.NavigationViewService.Initialize(navigationView);
        }
    }
}
