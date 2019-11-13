using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.ViewModels.Core
{
    public class DualModeViewModel<T>
        : BaseViewModel, IDualModeViewModel<T>
        where T : IObservableModel, new()
    {
        public DualModeViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner,
            IIndexViewModel<T> indexViewModel = null,
            ISelectedItemViewModel<T> selectedItemViewModel = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null,
            bool overrideSelectFirst = false)
            : base(commonParams, owner)
        {
            IndexViewModel = indexViewModel ?? 
                new IndexViewModel<T>(CommonParams);
            SelectedItemViewModel = selectedItemViewModel ??
                new SelectedItemViewModel<T>(CommonParams);

            if (dialogViewModel != null)
            {
                IndexViewModel.DialogViewModel =
                    SelectedItemViewModel.DialogViewModel = dialogViewModel;
            }

            OverrideSelectFirst = overrideSelectFirst;

            if (Owner == null)
            {
                Owner = this;
            }
            IndexViewModel.Owner = SelectedItemViewModel.Owner = Owner;
            HasFocus = Owner.HasFocus;

            MessengerInstance.Register<ModelObjectPersistedMessage<T>>(
                this, OnModelObjectPersisted);

            IndexViewModel.SelectedFirst += OnIndexViewModelSelectedFirst;
        }

        public override void Cleanup()
        {
            IndexViewModel.SelectedFirst -= OnIndexViewModelSelectedFirst;
            base.Cleanup();
        }

        public override bool HasFocus
        {
            get => base.HasFocus;
            set
            {
                base.HasFocus = value;
                if (IndexViewModel != null)
                {
                    IndexViewModel.HasFocus = value;
                }
                if (SelectedItemViewModel != null)
                {
                    SelectedItemViewModel.HasFocus = value;
                }
            }
        }

        public void ResetModelObjectToHeader()
        {
            SelectedItemViewModel.ResetModelObjectToHeader();
        }

        public async override Task<bool> LoadAsync()
        {
            return await IndexViewModel.LoadAsync();
        }

        public bool OverrideSelectFirst
        {
            get => IndexViewModel.OverrideSelectFirst;
            set => IndexViewModel.OverrideSelectFirst = value;
        }

        public IIndexViewModel<T> IndexViewModel { get; }

        public ISelectedItemViewModel<T> SelectedItemViewModel { get; set; }

        public virtual void Seed(
            IObservableModel parent, IEnumerable<T> index)
        {
            IndexViewModel?.Seed(index, parent);
            SelectedItemViewModel?.Seed(parent);
        }

        public void Clear()
        {
            IndexViewModel?.Clear();
            SelectedItemViewModel?.Clear();
        }

        public bool IsItemSelected => SelectedItemViewModel.IsItemSelected;

        private async void OnModelObjectPersisted(
            ModelObjectPersistedMessage<T> m)
        {
            if (HasFocus && m.Target == this)
            {
                if (SelectedItemViewModel?.Parent?.Id == m.Parent?.Id)
                {
                    var indexLoaded = false;
                    if (IndexViewModel != null)
                    {
                        indexLoaded = await IndexViewModel.UpdateIndexAsync(m);
                    }
                    if (!indexLoaded && SelectedItemViewModel != null)
                    {
                        if (m.IsDeletion)
                        {
                            if (SelectedItemViewModel.ModelObject != null)
                            {
                                SelectedItemViewModel.ModelObject = default;
                            }
                        }
                        else if (m.Updated != null &&
                            !m.Updated.Equals(SelectedItemViewModel.ModelObject))
                        {
                            SelectedItemViewModel.ModelObject = m.Updated;
                        }
                    }
                }
            }
        }

        private void OnIndexViewModelSelectedFirst(
            object sender, ISelectedItemEventArgs<T> e)
        {
            if (HasFocus && sender == IndexViewModel)
            {
                SelectedItemViewModel.ModelObject = e.SelectedItem;
            }
        }

    }
}
