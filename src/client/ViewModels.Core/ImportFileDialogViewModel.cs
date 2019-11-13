using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace savaged.mvvm.ViewModels.Core
{
    public class ImportFileDialogViewModel<T, R> 
        : BaseViewModel, IDragAndDropViewModel, INavigableDialogViewModel
        where T : IObservableModel
        where R : IObservableModel
    {
        private string _userFileLocation;
        private const string _importFileFilter = 
            "Comma Separated Values (*.csv)|*.csv|Excel (*.xlsx)|*.xlsx" +
            "|Open Office Calc (*.ods)|*.ods";
        private bool _isDirty;
        private readonly R _relation;
        private string _errors;
        private int _errorCount;

        public ImportFileDialogViewModel(
            IViewModelCommonParams commonParams, R relation)
            : base(commonParams)
        {
            _userFileLocation = string.Empty;
            GetUserFileLocationCmd = new RelayCommand(
                OnFormUserFileLocation, () => ViewState.IsNotBusy);
            FormSaveCmd = new RelayCommand(OnFormSave, () => CanSave);
            _relation = relation;
        }

        public override Task<bool> LoadAsync()
        {
            return AlwaysTrue();
        }

        public override bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }

        public RelayCommand GetUserFileLocationCmd { get; }

        public RelayCommand FormSaveCmd { get; }

        public string UserFileLocation
        {
            get => _userFileLocation;
            set
            {
                if (_userFileLocation != value)
                {
                    IsDirty = true;
                    RaisePropertyChanged(nameof(IsUserFileLocationPopluated));
                }
                Set(ref _userFileLocation, value);
            }
        }

        protected void OnFormUserFileLocation()
        {
            UserFileLocation = CommonParams?.FileHelper?.GetUserFileLocation(
                UserFileLocation, _importFileFilter);
        }

        public async Task<bool> FormSave()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            _errorCount = 0;

            if (!IsUserFileLocationPopluated)
            {
                MessengerInstance.Send(new BusyMessage(false, this));
                CloseView();
                return false;
            }
            var importFile = new FileInfo(UserFileLocation);
            
            await CommonParams.FileService.UploadAsync(
                AuthUser.Current, typeof(T), _relation, importFile);

            if (_errorCount == 0)
            {
                MessengerInstance.Send(new BusyMessage(false, this));
                ReactToSuccess("Imported");
                CloseView();
            }
            MessengerInstance.Send(new BusyMessage(false, this));
            return true;
        }

        protected override void ReactToException(object origin, DesktopException e)
        {
            _errorCount++;
            Errors += $" - {e.GetType().Name} :{Environment.NewLine}{e.Message}" +
                $"{Environment.NewLine}";
            MessengerInstance.Send(new BusyMessage(false, this));
        }
        public string Errors
        {
            get => _errors;
            private set => Set(ref _errors, value);
        }

        private bool IsUserFileLocationPopluated => !string.IsNullOrEmpty(UserFileLocation);

        public bool CanSave => ViewState.IsNotBusy && IsUserFileLocationPopluated;

        private async void OnFormSave()
        {
            await FormSave();
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            string filename = string.Empty;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // A file from Windows Explorer
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                // Just taking the first
                filename = files[0];

                var extension = Path.GetExtension(filename).ToLower();
                if (extension != ".csv" 
                    && extension != ".xls"
                    && extension != ".xlsx" 
                    && extension != ".ods")
                {
                    MessageBox.Show(
                        "Did you try to upload a non-data file?" +
                        "\nOnly these file types can be used:\n" +
                        _importFileFilter +
                        " \nNAUGHTY USER! \nGO AND THINK ABOUT WHAT YOU HAVE DONE!",
                        "What have you done now?",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(
                        "Did you try to upload an Outlook item?" +
                        " \nBAD USER! \nGO SIT ON THE NAUGHTY STEP!",
                        "What have you done now?",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UserFileLocation = filename;
        }

    }
}
