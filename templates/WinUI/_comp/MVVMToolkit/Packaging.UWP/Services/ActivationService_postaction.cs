//{[{
using Param_RootNamespace.Views;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
//}]}
namespace Param_RootNamespace.Services
{
    public class ActivationService : IActivationService
    {
        public async Task ActivateAsync(object activationArgs)
        {
//{--{
            // Initialize services that you need before app activation
            // take into account that the splash screen is shown while this code runs.
            await InitializeAsync();
//}--}
//{[{
            if (IsInteractive(activationArgs))
            {
                // Initialize services that you need before app activation
                // take into account that the splash screen is shown while this code runs.
                await InitializeAsync();

                if (Window.Current.Content == null)
                {
                    Window.Current.Content = Ioc.Default.GetService<Shell>();
                }
            }
//}]}
//{--{
            // Tasks after activation
            await StartupAsync();
//}--}
//{[{
            if (IsInteractive(activationArgs))
            {
                // Ensure the current window is active
                Window.Current.Activate();

                // Tasks after activation
                await StartupAsync();
            }
//}]}
        }

        private async Task HandleActivationAsync(object activationArgs)
        {
//{--{
            if (_defaultHandler.CanHandle(activationArgs))
            {
                await _defaultHandler.HandleAsync(activationArgs);
            }
//}--}
//{[{
            if (IsInteractive(activationArgs))
            {
                if (_defaultHandler.CanHandle(activationArgs))
                {
                    await _defaultHandler.HandleAsync(activationArgs);
                }
            }
//}]}
        }
//^^
//{[{

        private bool IsInteractive(object args)
        {
            // TODO: Verify is an interactive execution
            return true;
        }
//}]}
    }
}
