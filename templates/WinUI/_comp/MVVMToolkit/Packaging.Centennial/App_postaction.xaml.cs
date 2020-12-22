//{[{
using Param_RootNamespace.Contracts.Views;
//}]}

namespace Param_RootNamespace
{
    public partial class App : Application
    {
        private System.IServiceProvider ConfigureServices()
        {
            // Views and ViewModels
//{[{
            services.AddTransient<IShellWindow, Shell>();
//}]}
        }
    }
}

