using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using System.Windows;

namespace savaged.mvvm.ViewModels.Core
{
    public class DragAndDropDualModeViewModel<T, F>
        : DualModeViewModel<T>, IDragAndDropViewModel
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        public DragAndDropDualModeViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null,
            IIndexViewModel<T> indexViewModel = null,
            IDragDropSelectedItemViewModel<T> selectedItemViewModel = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null)
            : base(
                  commonParams, 
                  owner, 
                  indexViewModel, 
                  selectedItemViewModel) { }

        public IDragDropSelectedItemViewModel<T> DragDropSelectedItemViewModel
        {
            get
            {
                IDragDropSelectedItemViewModel<T> value = null;
                if (SelectedItemViewModel != null &&
                    SelectedItemViewModel is
                    IDragDropSelectedItemViewModel<T> ddVm)
                {
                    value = ddVm;
                }
                return value;
            }
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            DragDropSelectedItemViewModel.OnDrop(sender, e);
        }

    }
}
