using System;
using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class EmailAuthenticationProvider : AuthenticationProviderBase
    {
        public static string EmailProviderId = "Email";
        public static string EmailParameter = "Email";
        public static string PasswordParameter = "Password";

        public EmailAuthenticationProvider() : base(EmailProviderId)
        {
        }

        public override async Task<AuthenticationResult> AuthenticateAsync()
        {
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
