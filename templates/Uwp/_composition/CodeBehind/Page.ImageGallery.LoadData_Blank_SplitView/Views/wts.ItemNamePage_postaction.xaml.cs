namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNamePage : Page, INotifyPropertyChanged
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
                var selectedImageId = ImagesNavigationHelper.GetImageId(wts.ItemNameSelectedIdKey);
                if (!string.IsNullOrEmpty(selectedImageId))
                {
                    ImagesNavigationHelper.RemoveImageId(wts.ItemNameSelectedIdKey);
                }
            }
        }
        //}]}
    }
}