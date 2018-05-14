using System.Threading.Tasks;
using AppExtensionHost.Helpers;
using Windows.Foundation.Collections;

namespace AppExtensionHost.Services
{
    public class ExtensionRequest
    {
        private ValueSet _valueSet = new ValueSet();

        public ExtensionRequest()
        {
        }

        public ValueSet GetValueSet() => _valueSet;

        public async Task AddParameterAsync(string paramKey, object paramValue)
        {
            var stringValue = await Json.StringifyAsync(paramValue);
            _valueSet.Add(paramKey, stringValue);
        }
    }
}
