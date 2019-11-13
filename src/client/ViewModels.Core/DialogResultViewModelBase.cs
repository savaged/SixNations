using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils;
using System;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class DialogResultViewModelBase 
        : BaseViewModel, IDialogResultViewModel, INavigableDialogViewModel
    {
        public DialogResultViewModelBase(
            IViewModelCommonParams commonParams) 
            : base(commonParams)
        {
            HasFocus = true;
        }

        /// <summary>
        /// Override to return bool for Window event
        /// void OnClosing(object sender, CancelEventArgs e)
        /// {
        ///    if (DataContext is IDialogViewModel dialog)
        ///    {
        ///       e.Cancel = dialog.OnClosing();
        ///    }
        /// }
        /// </summary>
        /// <returns></returns>
        public virtual bool OnClosing(bool forceDialogResultSuccess = false)
        {
            var cancel = false;

            RaiseDialogClosed(forceDialogResultSuccess);

            return cancel;
        }

        private void RaiseDialogClosed(bool forceDialogResultSuccess)
        {
            var handler = DialogClosed;
            var dialogResult = forceDialogResultSuccess 
                ? true 
                : DialogResult;

            var args = new DialogClosedEventArgs(dialogResult);
            handler?.Invoke(this, args);
        }
        public event EventHandler<IDialogClosedEventArgs> DialogClosed = 
            delegate { };
    }
}
