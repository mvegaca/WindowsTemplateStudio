using System.Threading.Tasks;
using AppExtensionHost.Helpers;
using Windows.Foundation.Collections;

namespace AppExtensionHost.Services
{
    public class ExtensionResponse
    {
        private ValueSet _response;

        public ExtensionResponse(ValueSet response)
        {
            _response = response;
        }

        public async Task<T> GetValueAsync<T>(string valueKey)
        {
            if (_response.TryGetValue(valueKey, out var value))
            {
                if (value is string)
                {
                    return await Json.ToObjectAsync<T>(value as string);
                }
                else if (value is T)
                {
                    return (T)value;
                }
            }

            return default(T);
        }
    }
}
