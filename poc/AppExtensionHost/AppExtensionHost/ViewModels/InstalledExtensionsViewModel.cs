using System;
using System.Collections.ObjectModel;
using AppExtensionHost.Helpers;
using AppExtensionHost.Services;

namespace AppExtensionHost.ViewModels
{
    public class InstalledExtensionsViewModel : Observable
    {
        public ObservableCollection<Extension> Items => App.ExtensionsService.Extensions;

        public InstalledExtensionsViewModel()
        {
        }
    }
}
