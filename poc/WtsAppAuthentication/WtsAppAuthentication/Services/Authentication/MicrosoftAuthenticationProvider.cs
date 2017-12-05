using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataAccounts;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public class MicrosoftAuthenticationProvider : IAuthenticationProvider
    {
        public async Task<AuthenticationResult> AuthenticateAsync()
        {
            var result = new AuthenticationResult();
            try
            {
            }
            catch (Exception ex)
            {
                result.Success = false;
            }
            return result;
        }
    }
}
