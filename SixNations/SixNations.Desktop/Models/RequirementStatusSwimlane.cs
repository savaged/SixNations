using System;
using System.Collections.ObjectModel;

namespace SixNations.Desktop.Models
{
    public class RequirementStatusSwimlane
    {
        public RequirementStatusSwimlane()
        {
            Index = new ObservableCollection<Requirement>();
        }

        public ObservableCollection<Requirement> Index { get; }
    }
}
