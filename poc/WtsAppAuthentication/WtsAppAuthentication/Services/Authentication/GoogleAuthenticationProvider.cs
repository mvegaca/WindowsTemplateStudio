using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class GoogleAuthenticationProvider : IAuthenticationProvider
    {
        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
