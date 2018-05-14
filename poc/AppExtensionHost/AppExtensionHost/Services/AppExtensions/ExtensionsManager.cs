using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.UI.Core;

namespace AppExtensionHost.Services
{
    public class ExtensionsManager
    {
        private string _extensionContractName;
        private CoreDispatcher _dispatcher;
        private AppExtensionCatalog _catalog;

        public ObservableCollection<Extension> Extensions { get; } = new ObservableCollection<Extension>();

        public ExtensionsManager(string extensionContractName)
        {
            _extensionContractName = extensionContractName;
            _catalog = AppExtensionCatalog.Open(_extensionContractName);
            _dispatcher = null;
        }

        public async Task InitializeAsync()
        {
            await InitializeAsync(CoreWindow.GetForCurrentThread().Dispatcher);
        }

        public async Task InitializeAsync(CoreDispatcher dispatcher)
        {
            if (_dispatcher != null)
            {
                throw new Exception($"Extension Manager for {_extensionContractName} is already initialized.");
            }
            _dispatcher = dispatcher;
            _catalog.PackageInstalled += async (sender, args) => await LoadOrUpdateAppExtensionsAsync(args.Extensions);
            _catalog.PackageUpdated += async (sender, args) => await LoadOrUpdateAppExtensionsAsync(args.Extensions);
            _catalog.PackageUninstalling += OnPackageUninstalling;
            _catalog.PackageUpdating += async (sender, args) => await UnloadExtensionsAsync(args.Package);
            _catalog.PackageStatusChanged += OnPackageStatusChanged;

            var appExtensions = await _catalog.FindAllAsync();
            await LoadOrUpdateAppExtensionsAsync(appExtensions);
        }

        public async void RemoveExtension(Extension extension)
        {
            await _catalog.RequestRemovePackageAsync(extension.AppExtension.Package.Id.FullName);
        }

        private async void OnPackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var extensionsToRemove = Extensions.Where(ext => ext.AppExtension.Package.Id.FamilyName == args.Package.Id.FamilyName).ToList();
                extensionsToRemove.ForEach(ext =>
                {
                    ext.Unload();
                    Extensions.Remove(ext);
                });
            });
        }

        private async void OnPackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (args.Package.Status.VerifyIsOK())
                {
                    foreach (var extension in Extensions)
                    {
                        if (extension.IsFromPackage(args.Package))
                        {
                            extension.MarkAsLoaded();
                        }
                    }
                }
                else
                {
                    if (args.Package.Status.PackageOffline)
                    {
                        await UnloadExtensionsAsync(args.Package);
                    }
                    else if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                    {
                    }
                }
            });
        }        

        private async Task LoadOrUpdateAppExtensionsAsync(IEnumerable<AppExtension> appExtensions)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                foreach (var appExtension in appExtensions)
                {
                    if (appExtension.Package.Status.VerifyIsOK())
                    {
                        var existingExtension = Extensions.FirstOrDefault(e => e.IsFromAppExtension(appExtension));
                        if (existingExtension == null)
                        {
                            var newExtension = await Extension.FromAppExtensionAsync(appExtension);
                            Extensions.Add(newExtension);
                        }
                        else
                        {
                            existingExtension.Unload();
                            await existingExtension.UpdateAsync(appExtension);
                        }
                    }                    
                }
            });
        }

        private async Task UnloadExtensionsAsync(Package package)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var extension in Extensions)
                {
                    if (extension.IsFromPackage(package))
                    {
                        extension.Unload();
                    }
                }
            });
        }        
    }
}
