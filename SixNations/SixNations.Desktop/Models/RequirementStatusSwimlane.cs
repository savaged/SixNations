using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace SixNations.Desktop.Models
{
    public class RequirementStatusSwimlane : ObservableObject
    {
        public RequirementStatusSwimlane()
        {
            Index = new ObservableCollection<Requirement>();
        }

        public ObservableCollection<Requirement> Index { get; }
    }
}
