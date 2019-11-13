using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IAuthUserService
    {
        Task SetAuthUser(
            string email, string password, IAuthUser authUser);
    }
}