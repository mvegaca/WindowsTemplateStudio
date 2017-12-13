using System;
using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class GoogleAuthenticationProvider : AuthenticationProviderBase
    {
        public static string GoogleProviderId = "Google";

        public GoogleAuthenticationProvider() : base(GoogleProviderId)
        {
        }

        public override async Task<AuthenticationResult> AuthenticateAsync()
        {
            // TODO pending to implement with Google Console and Authentication Broker
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
