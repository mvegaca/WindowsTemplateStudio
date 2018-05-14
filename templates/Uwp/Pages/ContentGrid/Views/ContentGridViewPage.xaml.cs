using System;
using Windows.UI.Xaml.Controls;

namespace Param_ItemNamespace.Views
{
    public sealed partial class ContentGridViewPage : Page
    {
        //TODO : if you have advanced scenarios where you want to do functions like
        //Filter, Sort (on basis of specific properties of your item)
        // and incremental loading or other power features with a gridview/listview scenarios,
        //then you should have a look at https://docs.microsoft.com/en-us/windows/uwpcommunitytoolkit/helpers/advancedcollectionview
        //that will help you to create more advanced use cases for your collections.
        public ContentGridViewPage()
        {
            InitializeComponent();
        }
    }
}
