using System;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Param_ItemNamespace.Models;
using Param_ItemNamespace.Services;
namespace Param_ItemNamespace.Views
{
    public sealed partial class ContentGridViewPage : Page
    {
        /// <value>
        /// Source is the collection which uses INotifyPropertyChanged by default, so adding or removing
        /// items from source in c# will result in adding or removing items on UI automatically.
        /// </value>
        public ObservableCollection<SampleImage> Source => (new SampleDataService()).GetGallerySampleData();

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
