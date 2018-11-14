//{[{
using Windows.UI.Xaml;
//}]}
namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNamePage : Page, INotifyPropertyChanged
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
            var selectedImageId = ImagesNavigationHelper.GetImageId(wts.ItemNameSelectedIdKey);
            if (!string.IsNullOrEmpty(selectedImageId))
            {
                ImagesNavigationHelper.RemoveImageId(wts.ItemNameSelectedIdKey);
            }
        }
        //}]}
    }
}