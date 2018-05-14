using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AppExtensionHost.Helpers;
using AppExtensionHost.Services;

namespace AppExtensionHost.ViewModels
{
    public class ExtensionsViewModel : Observable
    {
        public readonly ObservableCollection<ExtensionViewModel> Extensions = new ObservableCollection<ExtensionViewModel>();

        public ExtensionsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            var request = await GetRequestAsync();
            Extensions.Clear();
            foreach (var ext in App.ExtensionsService.Extensions)
            {
                if (ext.IsEnabled)
                {
                    var extension = new ExtensionViewModel(ext);
                    await extension.InvokeAsync(request);
                    Extensions.Add(extension);
                }
            }
        }

        private async Task<ExtensionRequest> GetRequestAsync()
        {
            var request = new ExtensionRequest();
            await request.AddParameterAsync("Parameter01", "Hello World");
            await request.AddParameterAsync("Parameter02", 2018);
            await request.AddParameterAsync("Parameter03", new DateTime(2018, 3, 19));
            return request;
        }
    }
}
