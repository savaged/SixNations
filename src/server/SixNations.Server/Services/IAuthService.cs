using SixNations.Server.Models;
using System.Threading.Tasks;

namespace SixNations.Server.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
    }
}
