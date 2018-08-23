using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class RequirementDataService : IRequirementDataService
    {
        private IEnumerable<Requirement> Index()
        {
            throw new NotImplementedException();
        }

        // TODO: Get data from API
        public async Task<IEnumerable<Requirement>> GetModelDataAsync()
        {
            await Task.CompletedTask;

            return Index();
        }
    }
}
