namespace Param_ItemNamespace.Views
{
    public sealed partial class wts.ItemNameDetailPage : Page
    {
//{[{
        private static Frame _frame;

//}]}
        public wts.ItemNameDetailPage()
        {    
        }
//{[{

        public static void SetFrame(Frame frame)
        {
            _frame = frame;
        }
//}]}
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
//{[{
                _frame.SetListDataItemForNextConnectedAnnimation(ViewModel.SelectedImage);
//}]}
            }
        }
    }
}
