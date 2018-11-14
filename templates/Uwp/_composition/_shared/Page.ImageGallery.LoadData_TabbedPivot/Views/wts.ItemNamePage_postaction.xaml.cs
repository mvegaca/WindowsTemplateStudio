//{[{
using Windows.UI.Xaml;
using Param_ItemNamespace.Helpers;
//}]}
namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNamePage : Page
    {
        public wts.ItemNamePage()
        {
            //{[{
            Loaded += wts.ItemNamePage_Loaded;
            //}]}
        }

        //{[{
        private void wts.ItemNamePage_Loaded(object sender, RoutedEventArgs e)
        {
            ImagesNavigationHelper.RemoveImageId(wts.ItemNameViewModel.wts.ItemNameSelectedIdKey);
        }
        //}]}
    }
}