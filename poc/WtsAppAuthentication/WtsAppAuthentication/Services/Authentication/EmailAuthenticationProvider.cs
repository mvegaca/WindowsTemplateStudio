using System;
using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class EmailAuthenticationProvider : IAuthenticationProvider
    {
        private string _email;
        private string _password;

        public EmailAuthenticationProvider(string email, string password)
        {
            _email = email;
            _password = password;
        }

        public Task<AuthenticationResult> AuthenticateAsync()
        {
            return AuthenticateAsync(null);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(Action privacyPolicyInvokedAction)
        {
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
