using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

using AppExtensionHost.Helpers;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.AppService;

namespace AppExtensionHost.Services
{
    public class Extension
    {
        private const string ExtensionsStorageKey = "ExtensionsStorage";

        private PropertySet _properties;
        private string _serviceName;
        private ICommand _removeCommand;
        private ICommand _checkedCommand;
        private ICommand _uncheckedCommand;
        private readonly object _sync = new object();

        public AppExtension AppExtension { get; private set; }

        public string UniqueId { get; private set; }

        public BitmapImage Logo { get; private set; }


        public bool IsEnabled { get; private set; } = true;

        public bool IsLoaded { get; private set; }

        public bool IsOffline { get; private set; }

        public ICommand RemoveCommand => _removeCommand ?? (_removeCommand = new RelayCommand(OnRemove));

        public ICommand CheckedCommand => _checkedCommand ?? (_checkedCommand = new RelayCommand<RoutedEventArgs>((args) => CheckUpdatedAsync()));

        public ICommand UncheckedCommand => _uncheckedCommand ?? (_uncheckedCommand = new RelayCommand<RoutedEventArgs>((args) => CheckUpdatedAsync()));

        private Extension()
        {
        }

        public static async Task<Extension> FromAppExtensionAsync(AppExtension appExtension)
        {
            var extension = new Extension();
            await extension.LoadExtensionDataAsync(appExtension);
            return extension;
        }

        public async Task UpdateAsync(AppExtension appExtension)
        {
            if (IsFromAppExtension(appExtension))
            {
                await LoadExtensionDataAsync(appExtension);
            }
        }

        public bool IsFromPackage(Package package)
            => AppExtension.Package.Id.FamilyName == package.Id.FamilyName;

        public bool IsFromAppExtension(AppExtension appExtension)
            => UniqueId == GetUniqueId(appExtension);

        public void Unload()
        {
            lock (_sync)
            {
                if (IsLoaded)
                {
                    if (!AppExtension.Package.Status.VerifyIsOK() && !AppExtension.Package.Status.PackageOffline)
                    {
                        IsOffline = true;
                    }

                    IsLoaded = false;
                }
            }
        }

        public void MarkAsLoaded()
        {
            if (AppExtension.Package.Status.VerifyIsOK())
            {                
                if (!IsLoaded)
                {
                    IsLoaded = true;
                    IsOffline = false;
                }
            }
        }

        public async Task<ExtensionResponse> InvokeAsync(ExtensionRequest request)
        {
            if (IsLoaded)
            {
                try
                {
                    using (var connection = new AppServiceConnection())
                    {
                        connection.AppServiceName = _serviceName;
                        connection.PackageFamilyName = AppExtension.Package.Id.FamilyName;

                        var status = await connection.OpenAsync();
                        if (status == AppServiceConnectionStatus.Success)
                        {
                            var response = await connection.SendMessageAsync(request.GetValueSet());
                            if (response.Status == AppServiceResponseStatus.Success)
                            {
                                return new ExtensionResponse(response.Message);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Calling the App Service failed
                    return null;
                }
            }
            return null;
        }

        private async Task LoadExtensionDataAsync(AppExtension appExtension)
        {
            AppExtension = appExtension;
            UniqueId = GetUniqueId(appExtension);
            var fileStream = await (AppExtension.AppInfo.DisplayInfo.GetLogo(new Size(1, 1))).OpenReadAsync();
            BitmapImage logo = new BitmapImage();
            logo.SetSource(fileStream);
            Logo = logo;

            _properties = await AppExtension.GetExtensionPropertiesAsync() as PropertySet;

            _serviceName = null;
            if (_properties != null && _properties.ContainsKey("Service"))
            {
                PropertySet serviceProperty = _properties["Service"] as PropertySet;
                _serviceName = serviceProperty["#text"].ToString();
            }

            var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<List<ExtensionCacheProperties>>(ExtensionsStorageKey);
            cacheData = cacheData ?? (cacheData = new List<ExtensionCacheProperties>());
            var cacheItem = cacheData.FirstOrDefault(ecp => ecp.UniqueId == UniqueId);
            if (cacheItem == null)            
            {
                cacheData.Add(new ExtensionCacheProperties(UniqueId));
                await ApplicationData.Current.LocalFolder.SaveAsync(ExtensionsStorageKey, cacheData);
            }
            else
            {
                UpdateFromExtensionCacheProperties(cacheItem);
            }
            MarkAsLoaded();
        }

        private string GetUniqueId(AppExtension appExtension)
            => $"{appExtension.AppInfo.AppUserModelId}!{appExtension.Id}";

        private void OnRemove()
        {
            App.ExtensionsService.RemoveExtension(this);
        }

        private async Task CheckUpdatedAsync()
        {
            if (IsEnabled)
            {
                IsEnabled = false;
                await UpdateCachePropertiesAsync();
                Unload();
            }
            else
            {
                IsEnabled = true;
                await UpdateCachePropertiesAsync();
                MarkAsLoaded();
            }
        }

        private void UpdateFromExtensionCacheProperties(ExtensionCacheProperties cacheProperties)
        {
            IsEnabled = cacheProperties.IsEnabled;
        }

        private void UpdateExtensionCacheProperties(ExtensionCacheProperties cacheProperties)
        {
            cacheProperties.IsEnabled = IsEnabled;
        }

        private async Task UpdateCachePropertiesAsync()
        {
            var cacheData = await ApplicationData.Current.LocalFolder.ReadAsync<List<ExtensionCacheProperties>>(ExtensionsStorageKey);
            var cacheItem = cacheData.FirstOrDefault(ecp => ecp.UniqueId == UniqueId);
            UpdateExtensionCacheProperties(cacheItem);
            await ApplicationData.Current.LocalFolder.SaveAsync(ExtensionsStorageKey, cacheData);
        }
    }

    public class ExtensionCacheProperties
    {
        public readonly string UniqueId;

        public bool IsEnabled;

        public ExtensionCacheProperties(string uniqueId)
        {
            UniqueId = uniqueId;
        }
    }
}
