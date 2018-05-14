//{**
// This code block adds the method `GetSampleModelDataAsync()` to the SampleDataService of your project.
//**}
namespace Param_ItemNamespace.Services
{
    public static class SampleDataService
    {

//^^
//{[{
        // TODO WTS: Remove this once your ContentGrid page is displaying real data
        public static ObservableCollection<SampleOrder> GetContentGridData()
        {
            return new ObservableCollection<SampleOrder>(AllOrders());
        }
//}]}
    }
}