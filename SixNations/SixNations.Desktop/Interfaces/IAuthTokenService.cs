using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IAuthTokenService
    {
        Task<string> GetTokenAsync(string email, string password);
    }
}