using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SixNations.API.Interfaces;
using SixNations.Data.Facade;
using SixNations.Data.Models;
using SixNations.Data.Services;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Services;
using SixNations.Desktop.ViewModels;

namespace SixNations.Desktop.Test
{
    [TestClass]
    public class RequirementTests
    {
        private static RequirementViewModel _vm;

        [ClassInitialize]
        public static void InitOnce(TestContext context)
        {
            SimpleIoc.Default.Register<IHttpDataServiceFacade>(() =>
            {
                return new MockedHttpDataServiceFacade();
            });
            SimpleIoc.Default.Register<IDataService<Lookup>, LookupDataService>();
            SimpleIoc.Default.Register<IDataService<Requirement>, RequirementDataService>();

            SimpleIoc.Default.Register<INavigationService>(() =>
            {
                return new NavigationService();
            });
            SimpleIoc.Default.Register<MvvmDialogs.IDialogService>(() =>
            {
                return new MvvmDialogs.DialogService();
            });

            var mockConfirmation = new Mock<IActionConfirmationService>();
            mockConfirmation.Setup(c => c.Confirm(ActionConfirmations.Delete))
                .Returns(() => true);
            SimpleIoc.Default.Register(() =>
            {
                return mockConfirmation.Object;
            });

            SimpleIoc.Default.Register<BusyStateManager>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<RequirementViewModel>();

            User.Current.Initialise("mockToken");

            _vm = SimpleIoc.Default.GetInstance<RequirementViewModel>();
        }

        [TestInitialize]
        public async Task InitEach()
        {
            await _vm.LoadAsync();
            Assert.IsNotNull(_vm.Index);
        }

        [TestMethod]
        public void LoadTest()
        {
            Assert.AreEqual(6, _vm.Index.ToList().Count, 
                "The count of mocked requirements");
        }

        [TestMethod]
        public async Task NewCmdTest()
        {
            Assert.IsTrue(_vm.CanSelectItem, "CanSelectItem must be true");
            Assert.IsTrue(_vm.CanExecute, "CanExecute must be true");
            Assert.IsTrue(_vm.CanExecuteNew, "CanExecuteNew must be true");
            await Task.Run(() => _vm.NewCmd.Execute(null));
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");
            Assert.IsTrue(_vm.SelectedItem.IsNew, "The selected item must be new");
        }

        [TestMethod]
        public async Task EditCmdTest()
        {
            Assert.IsTrue(_vm.CanExecute, "CanExecute must be true");
            Assert.IsTrue(_vm.CanExecuteSelectedItemChange, 
                "CanExecuteSelectedItemChange must be true");
            await Task.Run(() => _vm.EditCmd.Execute(null));
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");
            Assert.IsFalse(_vm.SelectedItem.IsNew, "The selected item must not be new");
        }

        [TestMethod]
        public async Task SaveCmdTests()
        {
            // Setup the test data for a new model
            var dataService = SimpleIoc.Default.GetInstance<IDataService<Requirement>>();
            var testRequirement = await dataService.CreateModelAsync(
                User.Current.AuthToken, (ex) => throw ex);
            testRequirement.Story = "My test story";
            testRequirement.Estimation = 2;
            testRequirement.Priority = 3;
            testRequirement.Status = 4;
            testRequirement.Release = "Test release";
            _vm.SelectedItem = testRequirement;

            Assert.IsTrue(_vm.CanExecute, "CanExecute must be true");
            Assert.IsTrue(_vm.CanSelectItem, "CanSelectItem must be true");
            Assert.IsTrue(_vm.CanExecuteSave, "CanExecuteSave must be true");

            // The test action for saving a new model
            await Task.Run(() => _vm.SaveCmd.Execute(null));
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");
            Assert.IsFalse(_vm.SelectedItem.IsNew, "The selected item must not be new");
            Assert.AreEqual(testRequirement.Story, _vm.SelectedItem.Story, 
                "The selected item must have the 'edited' Story value");
            Assert.AreEqual(testRequirement.Estimation, _vm.SelectedItem.Estimation,
                "The selected item must have the 'edited' Estimation value");
            Assert.AreEqual(testRequirement.Priority, _vm.SelectedItem.Priority,
                "The selected item must have the 'edited' Priority value");
            Assert.AreEqual(testRequirement.Status, _vm.SelectedItem.Status,
                "The selected item must have the 'edited' Status value");
            Assert.AreEqual(testRequirement.Release, _vm.SelectedItem.Release,
                "The selected item must have the 'edited' Release value");

            // Setup the test for an existing model
            testRequirement = await dataService.EditModelAsync(
                User.Current.AuthToken, (ex) => throw ex, _vm.SelectedItem);
            _vm.SelectedItem = testRequirement;

            // The test action for saving an existing model
            await Task.Run(() => _vm.SaveCmd.Execute(null));
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");
            Assert.IsFalse(_vm.SelectedItem.IsNew, "The selected item must not be new");
            Assert.AreEqual(testRequirement.Story, _vm.SelectedItem.Story,
                "The selected item must have the 'edited' Story value");
            Assert.AreEqual(testRequirement.Estimation, _vm.SelectedItem.Estimation,
                "The selected item must have the 'edited' Estimation value");
            Assert.AreEqual(testRequirement.Priority, _vm.SelectedItem.Priority,
                "The selected item must have the 'edited' Priority value");
            Assert.AreEqual(testRequirement.Status, _vm.SelectedItem.Status,
                "The selected item must have the 'edited' Status value");
            Assert.AreEqual(testRequirement.Release, _vm.SelectedItem.Release,
                "The selected item must have the 'edited' Release value");
        }

        [TestMethod]
        public async Task DeleteCmdTest()
        {
            Assert.IsTrue(_vm.CanSelectItem, "CanSelectItem must be true");
            Assert.IsTrue(_vm.CanExecute, "CanExecute must be true");
            Assert.IsTrue(_vm.CanExecuteSelectedItemChange, 
                "CanExecuteSelectedItemChange must be true");
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");

            var deleteMeId = _vm.SelectedItem.Id;

            await Task.Run(() => _vm.DeleteCmd.Execute(null));
            Assert.IsNotNull(_vm.SelectedItem, "The selected item must not be null");
            Assert.AreNotEqual(deleteMeId, _vm.SelectedItem.Id, 
                "The selected item must not be the same as the one deleted");
        }
    }
}
