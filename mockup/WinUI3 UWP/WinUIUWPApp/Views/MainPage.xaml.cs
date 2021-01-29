using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using WinUIUWPApp.ViewModels;

namespace WinUIUWPApp.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        public MainPage()
        {
            ViewModel = Ioc.Default.GetService<MainViewModel>();
            InitializeComponent();
        }
    }
}
