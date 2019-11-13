using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IUserLookupService : ILookupService
    {
        Task<ILookup> GetAsync(
            IAuthUser user, IDataModel relation, string relationUserField);

        Task<ILookup> GetAsync(IAuthUser user);
    }
}