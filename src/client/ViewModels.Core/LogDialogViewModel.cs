using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace savaged.mvvm.ViewModels.Core
{
    public class LogDialogViewModel<TLoggedModel>
        : BaseViewModel, ILogDialogViewModel<TLoggedModel>
        where TLoggedModel : IObservableModel, new()
    {
        private TLoggedModel _loggedModel;
        private TLoggedModel _modelLog;

        public LogDialogViewModel(
            IViewModelCommonParams commonParams,
            ILoggedViewModel<TLoggedModel> owner)
            : base(commonParams, owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }
            Index = new ObservableCollection<IModelLog>();
        }

        public void Seed(TLoggedModel loggedModel)
        {
            var tmp = loggedModel.Clone();
            _loggedModel = tmp;
        }

        public async override Task<bool> LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var index = await ((ILoggedViewModel<TLoggedModel>)Owner)
                    .LoadModelLogIndexAsync();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (var model in index)
                    {
                        Index.Add(model);
                    }
                });
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return true;
        }

        public ObservableCollection<IModelLog> Index { get; private set; }

        public TLoggedModel ModelObject
        {
            get => _modelLog;
            set => Set(ref _modelLog, value);
        }
        
    }
}
