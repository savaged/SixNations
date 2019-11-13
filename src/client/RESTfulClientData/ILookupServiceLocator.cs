using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface ILookupServiceLocator
    {
        IDynamicLookupService GetDynamicLookupService();
        Task<IStaticLookupService> GetStaticLookupServiceAsync(
            IAuthUser user);
        IStaticLookupService GetStaticLookupService();
        IUserInputLookupService GetUserInputLookupService();
        IUserLookupService GetUserLookupService();
        ILookupAdminService GetLookupAdminService();
    }
}