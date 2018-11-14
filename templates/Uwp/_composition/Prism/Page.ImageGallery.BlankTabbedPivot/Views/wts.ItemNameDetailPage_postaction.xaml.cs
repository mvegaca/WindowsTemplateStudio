//{[{
using Windows.UI.Xaml;
//}]}
namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNameDetailPage : Page
    {
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
//{[{
                if (Window.Current.Content is Frame frame)
                {
                    frame.SetListDataItemForNextConnectedAnnimation(ViewModel.SelectedImage);
                }
//}]}
            }
        }
    }
}
