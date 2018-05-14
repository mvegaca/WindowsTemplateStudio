using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppExtensionHost.Helpers;
using AppExtensionHost.Services;

namespace AppExtensionHost.ViewModels
{
    public class ExtensionViewModel : Observable
    {
        private readonly Extension _extension;

        public ExtensionViewModel(Extension extension)
        {
            _extension = extension;
        }

        public async Task InvokeAsync(ExtensionRequest request)
        {
            var response = await _extension.InvokeAsync(request);
            var response01 = await response.GetValueAsync<string>("Response01");
            var response02 = await response.GetValueAsync<int>("Response02");
            var response03 = await response.GetValueAsync<DateTime>("Response03");
        }
    }
}
