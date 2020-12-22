//{[{
using Param_RootNamespace.Contracts.Views;
//}]}
namespace Param_RootNamespace.Services
{
    public class ActivationService : IActivationService
    {
        public ActivationService(/*{[{*/IShellWindow shellWindow/*}]}*/)
        {
//^^
//{[{
            App.MainWindow = shellWindow as Window;
//}]}
        }

        public async Task ActivateAsync(object activationArgs)
        {
            await InitializeAsync();
//{[{

            App.MainWindow.Activate();
//}]}
        }
    }
}