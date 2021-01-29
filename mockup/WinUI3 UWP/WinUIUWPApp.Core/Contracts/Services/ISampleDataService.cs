using System.Collections.Generic;
using System.Threading.Tasks;

using WinUIUWPApp.Core.Models;

namespace WinUIUWPApp.Core.Contracts.Services
{
    // Remove this class once your pages/features are using your data.
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetContentGridDataAsync();

        Task<IEnumerable<SampleOrder>> GetGridDataAsync();

        Task SaveOrderAsync(SampleOrder order);

        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
