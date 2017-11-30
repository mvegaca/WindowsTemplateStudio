using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public static class AuthenticationService
    {
        private static IAuthenticationProvider _provider;

        public static void Initialize(IAuthenticationProvider provider)
        {
            _provider = provider;
        }

        public static async Task<AuthenticationResult> AuthenticateAsync()
        {
            return await _provider.AuthenticateAsync();
        }
    }
}
