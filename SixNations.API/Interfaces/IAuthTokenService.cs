using System.Threading.Tasks;

namespace SixNations.API.Interfaces
{
    public interface IAuthTokenService
    {
        Task<string> GetTokenAsync(string email, string password);
    }
}