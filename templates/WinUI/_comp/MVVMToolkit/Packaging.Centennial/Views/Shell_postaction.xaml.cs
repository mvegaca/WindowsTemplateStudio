//{[{
using Param_RootNamespace.Helpers;
//}]}

namespace Param_RootNamespace.Views
{
    public sealed partial class Shell : SHELL_TYPE
    {
        public Shell(ShellViewModel viewModel)
        {
//{[{
            Title = "AppDisplayName".GetLocalized();
//}]}
        }
    }
}
