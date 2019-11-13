using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using System;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class BaseModelObjectViewModel<T> 
        : BaseViewModel, IModelObjectViewModel<T>
        where T : IObservableModel, new()
    {
        public BaseModelObjectViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null)
            : base(commonParams, owner)
        {
            DialogViewModel = dialogViewModel;
        }

        public IObservableModel Parent { get; set; }

        public IModelObjectDialogViewModel<T> DialogViewModel
        { get; set; }

        public abstract void Clear();

        protected void ShowDialog(T modelObject = default)
        {
            if (DialogViewModel == null)
            {
                throw new InvalidOperationException(
                    "The dialog view model should be set by here!");
            }
            var vm = DialogViewModel;

            var isModal = NavigationService.DialogService.IsModal(vm);

            if (!isModal)
            {
                vm = DialogViewModel.Template();
                if (vm == null || vm == DialogViewModel)
                {
                    throw new InvalidOperationException(
                        "There should be a new copy of the dialog view model!");
                }
                if (vm.Owner != DialogViewModel.Owner)
                {
                    throw new InvalidOperationException(
                        "The dialog view model should have the same " +
                        "owner as the original dialog view model!");
                }
            }
            vm.HasFocus = true;
            vm.Seed(modelObject, Parent);
            NavigationService.DialogService.Show(vm);
        }

    }
}
