//{[{
using Windows.UI.Xaml.Navigation;
using Param_ItemNamespace.Helpers;
//}]}
namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNamePage : Page
    {
        public wts.ItemNamePage()
        {
        }

        //{[{
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ImagesNavigationHelper.RemoveImageId(wts.ItemNameViewModel.wts.ItemNameSelectedIdKey);
            }
        }
        //}]}
    }
}