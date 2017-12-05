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

        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            await Task.CompletedTask;
            return new AuthenticationResult() { Success = true };
        }
    }
}
