using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;

using Microsoft.Toolkit.Uwp.UI.Controls;

using SixNations.Models;
using SixNations.Services;

namespace SixNations.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Requirement _selected;

        public Requirement Selected
        {
            get { return _selected; }
            set { Set(ref _selected, value); }
        }

        public ObservableCollection<Requirement> Index { get; private set; } = new ObservableCollection<Requirement>();

        public MainViewModel()
        {
        }

        public async Task LoadDataAsync(MasterDetailsViewState viewState)
        {
            Index.Clear();

            var data = await RequirementDataService.GetModelDataAsync();

            foreach (var item in data)
            {
                Index.Add(item);
            }

            if (viewState == MasterDetailsViewState.Both)
            {
                Selected = Index.First();
            }
        }
    }
}
