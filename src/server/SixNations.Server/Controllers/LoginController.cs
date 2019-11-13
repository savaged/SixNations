using Microsoft.AspNetCore.Mvc;
using SixNations.Server.Models;
using SixNations.Server.Services;
using System.Threading.Tasks;

namespace SixNations.Server.Controllers
{
    // TODO set-up and integrate a standard identity server like identityserver4 or
    // something like the ASP.Net Core auth demo:
    // https://github.com/aspnet/Docs/tree/master/aspnetcore/security/authentication/identity/sample/src/ASPNETCore-IdentityDemoComplete/IdentityDemo
    // Then remove all my 'home-baked' auth stuff
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _auth;

        public LoginController(IAuthService auth)
        {
            _auth = auth;
        }

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> PostLogin(
            [FromForm] string username,
            [FromForm] string password)
        {
            var token = await _auth.AuthenticateAsync(username, password);            
            var root = new ResponseRootObject(200, token);
            return Ok(root);
        }
    }
}