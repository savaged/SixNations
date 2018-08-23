using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Interfaces
{
    public interface IRequirementDataService
    {
        Task<IEnumerable<Requirement>> GetModelDataAsync();
    }
}