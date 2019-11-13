using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class DualModeDialogViewModel<T>
        : ModelObjectDialogViewModel<T>, IDualModeDialogViewModel<T> 
        where T : IObservableModel, new()
    {
        private bool _formVisible;

        public DualModeDialogViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner)
            : base(commonParams, owner)
        {
            IndexViewModel = new IndexViewModel<T>(CommonParams, this);

            IndexViewModel.AddCmd = new RelayCommand(
                OnAdd, () => IndexViewModel.CanAdd);

            IndexViewModel.HasFocus = HasFocus;

            OverrideSelectFirst = true;
        }

        public override async Task<bool> LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                if (IndexVisible)
                {
                    await IndexViewModel.LoadAsync();
                }
                else
                {
                    await base.LoadAsync();
                }
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return true;
        }

        public override void Seed(IObservableModel parent)
        {
            base.Seed(parent);
            IndexViewModel?.Seed(parent);
        }

        public virtual void Seed(IObservableModel parent, IEnumerable<T> index)
        {
            base.Seed(parent);
            IndexViewModel?.Seed(index, parent);
        }

        public bool IsAdding
        {
            get => FormVisible;
            set => FormVisible = value;
        }

        public bool FormVisible
        {
            get => _formVisible;
            set
            {
                Set(ref _formVisible, value);
                RaisePropertyChanged(nameof(IndexVisible));
            }
        }

        public bool IndexVisible => !FormVisible;

        public IIndexViewModel<T> IndexViewModel { get; }

        public ISelectedItemViewModel<T> SelectedItemViewModel
        {
            get => this;
            set => throw new NotSupportedException(
                $"This class is an instance of {nameof(SelectedItemViewModel)}");
        }

        public bool OverrideSelectFirst
        {
            get => IndexViewModel.OverrideSelectFirst;
            set => IndexViewModel.OverrideSelectFirst = value;
        }

        public override void Clear()
        {
            base.Clear();
            IndexViewModel.Clear();
        }

        protected virtual async Task Add()
        {
            ModelObject = default;
            FormVisible = true;
            await LoadAsync();
        }

        protected async override Task ReactToSuccessfulSave()
        {
            IsDirty = false;
            await IndexViewModel?.LoadAsync();
        }

        private async void OnAdd()
        {
            await Add();
        }

    }
}
