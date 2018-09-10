using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Models;
using SixNations.Desktop.Constants;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using SixNations.Desktop.Adapters;
using System.IO;
using CommonServiceLocator;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;

namespace SixNations.Desktop.ViewModels
{
    public class RequirementViewModel 
        : DataBoundViewModel<Requirement>, IParameterised
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDataService<Lookup> _lookupDataService;
        private Lookup _estimationLookup;
        private Lookup _priorityLookup;
        private Lookup _statusLookup;
        private string _storyFilter;
        private Requirement _preLoaded;
        private readonly IIndexExcelAdapter<Requirement> _excelAdapter;

        public RequirementViewModel(
            IDataService<Requirement> requirementDataService,
            IDataService<Lookup> lookupService)
            : base(requirementDataService)
        {
            _storyFilter = string.Empty;

            _lookupDataService = lookupService;

            _excelAdapter = new IndexExcelAdapter<Requirement>();
            IndexToExcelCmd = new RelayCommand(OnIndexToExcel, () => _excelAdapter.CanExecute);
            ExcelToIndexCmd = new RelayCommand(OnExcelToIndex, () => _excelAdapter.CanExecute);

            MessengerInstance.Register<StoryFilterMessage>(this, OnFindStory);
        }

        public void Initialise(object parameter)
        {
            if (parameter != null && parameter is Requirement requirement)
            {
                _preLoaded = requirement;
            }
        }

        public ICommand StoryFilterCmd { get; }

        public ICommand ClearStoryFilterCmd { get; }

        public ICommand IndexToExcelCmd { get; }

        public ICommand ExcelToIndexCmd { get; }

        public override async Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            await base.LoadAsync();
            if (_preLoaded != null)
            {
                SelectedItem = Index.Where(r => r.Id == _preLoaded.Id).FirstOrDefault();
            }
            else
            {
                SelectedItem = Index.FirstOrDefault();
            }
            try
            {
                await LoadLookupAsync();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unexpexted exception loading! {0}", ex);
                FeedbackActions.ReactToException(ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async Task LoadLookupAsync()
        {
            var lookups = await _lookupDataService.GetModelDataAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException);

            EstimationLookup = lookups.First(l => l.Name == "RequirementEstimation");
            PriorityLookup = lookups.First(l => l.Name == "RequirementPriority");
            StatusLookup = new Lookup(RequirementStatus._);
        }

        public Lookup EstimationLookup
        {
            get => _estimationLookup;
            private set => Set(ref _estimationLookup, value);
        }

        public Lookup PriorityLookup
        {
            get => _priorityLookup;
            private set => Set(ref _priorityLookup, value);
        }

        public Lookup StatusLookup
        {
            get => _statusLookup;
            private set => Set(ref _statusLookup, value);
        }

        public string StoryFilter
        {
            get => _storyFilter;
            set => Set(ref _storyFilter, value);
        }

        private void OnFindStory(StoryFilterMessage m)
        {
            StoryFilter = m.Filter;
        }

        private void OnIndexToExcel()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            _excelAdapter.Adapt(Index);
            MessengerInstance.Send(new BusyMessage(false, this));
        }

        private void OnExcelToIndex()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            FileInfo file = null;
            var fileDialogInfo = new OpenFileDialogSettings
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Excel (*.xlsx)|*.xlsx|Legacy Excel (*.xls)|*.xls| Open Format (*.ods)|*.ods"
            };
            var dialogService = ServiceLocator.Current.GetInstance<IDialogService>();
            var owner = ServiceLocator.Current.GetInstance<MainViewModel>();
            var success = dialogService.ShowOpenFileDialog(owner, fileDialogInfo);
            if (success == true)
            {
                var path = fileDialogInfo.FileName;
                file = new FileInfo(path);
                _excelAdapter.Adapt(file);
            }            
            MessengerInstance.Send(new BusyMessage(false, this));
        }
    }
}
