using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface ILookupAdminService
    {
        Task<bool> CanAdd(IAuthUser user, string lookupName);
        Task<bool> CanEdit(IAuthUser user, string lookupName, int key);
        Task Store(IAuthUser user, string lookupName, string value);
        Task Update(IAuthUser user, string lookupName, int key, string value);
    }
}