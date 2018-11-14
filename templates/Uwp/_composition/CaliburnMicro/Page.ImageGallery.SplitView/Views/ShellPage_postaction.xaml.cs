namespace Param_ItemNamespace.Views
{
    public sealed partial class ShellPage : IShellView
    {
        public INavigationService CreateNavigationService(WinRTContainer container)
        {
//^^
//{[{
            wts.ItemNameDetailPage.SetFrame(shellFrame);
//}]}
            return navigationService;
        }
    }
}
