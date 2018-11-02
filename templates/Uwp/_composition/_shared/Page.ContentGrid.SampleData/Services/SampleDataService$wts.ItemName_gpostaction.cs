﻿//{**
// This code block adds the method `GetContentGridData()` to the SampleDataService of your project.
//**}
//{[{
using System.Threading.Tasks;
//}]}
namespace Param_ItemNamespace.Services
{
    public static class SampleDataService
    {
//{[{
        private static IEnumerable<SampleOrder> _allOrders;
//}]}

//^^
//{[{

        // TODO WTS: Remove this once your ContentGrid page is displaying real data
        public static ObservableCollection<SampleOrder> GetContentGridData()
        {
            if (_allOrders == null)
            {
                _allOrders = AllOrders();
            }

            return new ObservableCollection<SampleOrder>(_allOrders);
        }
//}]}
    }
}
