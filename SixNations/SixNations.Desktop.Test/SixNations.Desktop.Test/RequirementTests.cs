using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SixNations.API.Interfaces;
using SixNations.Data.Facade;
using SixNations.Data.Models;
using SixNations.Data.Services;

namespace SixNations.Desktop.Test
{
    [TestClass]
    public class RequirementTests
    {
        private static IDataService<Requirement> _requirementsDataService;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            SimpleIoc.Default.Register<IHttpDataServiceFacade>(() =>
            {
                return new MockedHttpDataServiceFacade();
            });
            SimpleIoc.Default.Register<IDataService<Lookup>, LookupDataService>();
            SimpleIoc.Default.Register<IDataService<Requirement>, RequirementDataService>();

            _requirementsDataService = SimpleIoc.Default
                .GetInstance<IDataService<Requirement>>();
        }

        [TestMethod]
        public async Task IndexTest()
        {
            var index = await _requirementsDataService.GetModelDataAsync(
                "mockToken", (ex) => throw ex);
            Assert.IsNotNull(index);
            Assert.AreEqual(6, index.ToList().Count, "The count of mocked requirements");
        }
    }
}
