using System;
using System.Collections.ObjectModel;
using Param_ItemNamespace.Models;
using Param_ItemNamespace.Services;
namespace Param_ItemNamespace.ViewModels
{
    public class ContentGridViewViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        /// <value>
        /// Source is the collection which uses INotifyPropertyChanged by default, so adding or removing
        /// items from source in c# will result in adding or removing items on UI automatically.
        /// </value>
        public ObservableCollection<SampleImage> Source => (new SampleDataService()).GetGallerySampleData();
    }
}
