using System.Threading.Tasks;

namespace WinUIUWPApp.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);

        Task HandleAsync(object args);
    }
}
