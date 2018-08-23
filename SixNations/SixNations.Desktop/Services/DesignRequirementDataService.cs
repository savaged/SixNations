using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class DesignRequirementDataService : IRequirementDataService
    {
        private IEnumerable<Requirement> Index()
        {
            // The following is order summary data
            var data = new ObservableCollection<Requirement>
            {
                new Requirement
                {
                    RequirementID = 75,
                    Story = "As a {user role}, I want to {the thing that is needed}, so that {the business value that allows prioritisation}.",
                    EstimationName = "XXL",
                    PriorityName = "Could",
                    StatusName = "WIP",
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 76,
                    Story = "As a standard user, I want to see savings for the previous day and week, so that {need the reason}.",
                    EstimationName = "Medium",
                    PriorityName = "Should",
                    StatusName = "Prioritised",
                    Release = ""
                },
                new Requirement
                {
                    RequirementID = 77,
                    Story = "As an Admin user, I want to add and edit roles, so that permissions can be managed more easily.",
                    EstimationName = "Large",
                    PriorityName = "Must",
                    StatusName = "Test",
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 78,
                    Story = "As an Admin user, I want to add and remove users, so that permissions can be managed more easily.",
                    EstimationName = "XL",
                    PriorityName = "Must",
                    StatusName = "Done",
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 79,
                    Story = "As an authorised user, I want to change my password, in case I have forgotten it, or so that I comply with the security policy to change it bi-monthly.",
                    EstimationName = "XS",
                    PriorityName = "Could",
                    StatusName = "Done",
                    Release = "R1"
                },
                new Requirement
                {
                    RequirementID = 79,
                    Story = "As a standard user, I want to add or edit email recipients, so that {need a reason here}.",
                    EstimationName = "Small",
                    PriorityName = "Wont",
                    StatusName = "Done",
                    Release = ""
                }
            };

            return data;
        }

        // TODO: Get data from API
        public async Task<IEnumerable<Requirement>> GetModelDataAsync()
        {
            await Task.CompletedTask;

            return Index();
        }
    }
}
