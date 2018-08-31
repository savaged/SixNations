using System;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(IDataService<Requirement> dataService) 
            : base(dataService)
        {
        }
    }
}
