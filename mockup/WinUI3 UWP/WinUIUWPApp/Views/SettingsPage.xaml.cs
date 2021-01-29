using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

using WinUIUWPApp.ViewModels;

namespace WinUIUWPApp.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            ViewModel = Ioc.Default.GetService<SettingsViewModel>();
            InitializeComponent();
        }
    }
}
