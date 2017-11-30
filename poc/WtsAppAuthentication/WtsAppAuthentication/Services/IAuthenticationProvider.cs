using System.Threading.Tasks;
using WtsAppAuthentication.Models;

namespace WtsAppAuthentication.Services
{
    public interface IAuthenticationProvider
    {
        Task<AuthenticationResult> AuthenticateAsync();
    }
}
