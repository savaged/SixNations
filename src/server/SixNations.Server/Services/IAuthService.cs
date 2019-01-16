using SixNations.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.Server.Services
{
    public interface IAuthService
    {
        Task<Token> AuthenticateAsync(string username, string password);
    }
}
