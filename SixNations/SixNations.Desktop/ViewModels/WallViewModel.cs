using System;
using System.Collections.ObjectModel;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(IDataService<Requirement> dataService) 
            : base(dataService)
        {
            Prioritised = new ObservableCollection<Requirement>();
            WIP = new ObservableCollection<Requirement>();
            Test = new ObservableCollection<Requirement>();
            Done = new ObservableCollection<Requirement>();
        }

        public ObservableCollection<Requirement> Prioritised { get; }

        public ObservableCollection<Requirement> WIP { get; }

        public ObservableCollection<Requirement> Test { get; }

        public ObservableCollection<Requirement> Done { get; }
    }
}
