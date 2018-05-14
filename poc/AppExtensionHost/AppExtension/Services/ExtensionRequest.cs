using System.Threading.Tasks;
using AppExtension.Helpers;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace AppExtension.Services
{
    public class ExtensionRequest
    {
        private ValueSet _parameters = new ValueSet();

        public ExtensionRequest(AppServiceRequest request)
        {
            _parameters = request.Message;
        }

        public async Task<T> GetParameterAsync<T>(string valueKey)
        {
            if (_parameters.TryGetValue(valueKey, out var value))
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
