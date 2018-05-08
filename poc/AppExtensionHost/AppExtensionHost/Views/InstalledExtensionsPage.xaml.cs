using System;

using AppExtensionHost.ViewModels;

using Windows.UI.Xaml.Controls;

namespace AppExtensionHost.Views
{
    public sealed partial class InstalledExtensionsPage : Page
    {
        public InstalledExtensionsViewModel ViewModel { get; } = new InstalledExtensionsViewModel();

        public InstalledExtensionsPage()
        {
            InitializeComponent();
        }
    }
}
