using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class DesignRequirementDataService : IHttpDataService<Requirement>
    {
        private IEnumerable<Requirement> Index()
        {
            var data = new ObservableCollection<Requirement>
            {
                new Requirement
                {
                    RequirementID = 75,
                    Story = "As a {user role}, I want to {the thing that is needed}, so that {the business value that allows prioritisation}.",
                    Estimation = 13,
                    Priority = 3,
                    Status = 2,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 76,
                    Story = "As a standard user, I want to see savings for the previous day and week, so that {need the reason}.",
                    Estimation = 3,
                    Priority = 2,
                    Status = 1,
                    Release = ""
                },
                new Requirement
                {
                    RequirementID = 77,
                    Story = "As an Admin user, I want to add and edit roles, so that permissions can be managed more easily.",
                    Estimation = 5,
                    Priority = 1,
                    Status = 3,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 78,
                    Story = "As an Admin user, I want to add and remove users, so that permissions can be managed more easily.",
                    Estimation = 8,
                    Priority = 1,
                    Status = 4,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 79,
                    Story = "As an authorised user, I want to change my password, in case I have forgotten it, or so that I comply with the security policy to change it bi-monthly.",
                    Estimation = 1,
                    Priority = 3,
                    Status = 4,
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 79,
                    Story = "As a standard user, I want to add or edit email recipients, so that {need a reason here}.",
                    Estimation = 2,
                    Priority = 4,
                    Status = 4,
                    Release = ""
                }
            };
            return data;
        }

        public async Task<IEnumerable<Requirement>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            await Task.CompletedTask;

            return Index();
        }
    }
}
