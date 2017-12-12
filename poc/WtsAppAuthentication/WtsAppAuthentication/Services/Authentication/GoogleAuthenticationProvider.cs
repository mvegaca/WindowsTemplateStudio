using System;
using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class GoogleAuthenticationProvider : IAuthenticationProvider
    {
        public Task<AuthenticationResult> AuthenticateAsync()
        {
            return AuthenticateAsync(null);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(Action privacyPolicyInvokedAction)
        {
            // TODO pending to implement with Google Console and Authentication Broker
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
