using System.Threading.Tasks;

namespace WinUIUWPApp.Contracts.Services
{
    public interface IActivationService
    {
        Task ActivateAsync(object activationArgs);
    }
}
