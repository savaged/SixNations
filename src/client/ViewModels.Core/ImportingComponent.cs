using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.ViewModels.Core.Utils;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class ImportingComponent<T, C> 
        : ViewModelBase, IImporting<T, C>
        where T : IObservableModel
        where C : IChildCollection<T>, new()
    {
        private readonly IImporting<T, C> _owner;
        private readonly ExceptionHandlingService _exceptionHandler;

        public ImportingComponent(
            IImporting<T, C> owner)
        {
            _owner = owner;

            _exceptionHandler = new ExceptionHandlingService();

            ClearCmd = new RelayCommand(
                OnClear, () => CanExecuteClear);
            PasteCmd = new RelayCommand(OnPaste);
            FormSaveCmd = new RelayCommand(OnFormSave, () => CanExecuteSave);
        }

        public IViewModelCommonParams CommonParams => 
            _owner.CommonParams;

        public void CloseView(bool result = true) => _owner.CloseView(result);

        public IObservableModel Parent => _owner.Parent;

        public C Import => _owner.Import;

        public ObservableCollection<T> Index => _owner.Index;

        public bool CanExecuteClear => Index.Count > 0;

        public bool CanExecuteSave => Index.Count > 0;

        public bool CanSave => IsImportValid;

        public ICommand FormSaveCmd { get; }

        public ICommand ClearCmd { get; }

        public ICommand PasteCmd { get; }
        public bool IsImportValid { get; set; }

        public async Task UpdateIndexFromDataTable(DataTable dt)
        {
            await _owner.UpdateIndexFromDataTable(dt);
        }

        public async Task<bool> FormSave()
        {
            if (!IsImportValid)
            {
                return false;
            }
            Import.ParentId = Parent != null ? Parent.Id : 0;
            Import.Children = Index;

            C saved = default;
            try
            {
                saved = await CommonParams.ModelService.StoreAsync(
                    AuthUser.Current, Import);
            }
            catch (ApiValidationException ex)
            {
                _exceptionHandler.ReactToException(this, ex);
                return false;
            }
            catch (ApiAuthException)
            {
                return false;
            }
            catch (DesktopException ex)
            {
                _exceptionHandler.ReactToException(this, ex);
            }
            if (saved != null && string.IsNullOrEmpty(saved.Error))
            {
                MessageBox.Show(
                    $"Imported {_owner.Index.Count} records.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(
                    $"No confirmation returned for import of {_owner.Index.Count}" +
                    "records. Please manually confirm import success.",
                    "Warning",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            MessengerInstance.Send(new ImportCompletedMessage<T>(Import));
            return true;
        }

        private void OnClear()
        {
            Clear();
        }

        private void Clear()
        {
            Clipboard.Clear();
            _owner.Index.Clear();            
        }

        private async void OnPaste()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                IsImportValid = false;
                var csv = Clipboard.GetText(
                    TextDataFormat.CommaSeparatedValue);
                await UpdateIndexFromCsv(csv);
                IsImportValid = true;
            }
            catch (DesktopException ex)
            {
                _exceptionHandler.ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async Task UpdateIndexFromCsv(string csv)
        {
            var dt = await Task.Run(() =>
            CsvToDataTableConverter.GetDataTableFromCsv(csv));

            await UpdateIndexFromDataTable(dt);
        }

        private async void OnFormSave()
        {
            var result = await _owner.FormSave();
            if (result)
            {
                CloseView(result);
            }
        }
    }
}
