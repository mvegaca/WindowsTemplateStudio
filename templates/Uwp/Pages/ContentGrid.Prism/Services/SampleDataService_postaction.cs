//{**
// This code block adds the method `GetContentGridData()` to the SampleDataService of your project.
//**}
//{[{
using System.Threading.Tasks;
//}]}
namespace Param_ItemNamespace.Services
{
    public class SampleDataService : ISampleDataService
    {
//^^
//{[{
        // TODO WTS: Remove this once your ContentGrid page is displaying real data
        public ObservableCollection<SampleOrder> GetContentGridData()
        {
            return new ObservableCollection<SampleOrder>(AllOrders());
        }
//}]}
    }
}
