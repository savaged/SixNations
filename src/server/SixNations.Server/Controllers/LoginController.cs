using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixNations.Server.Data;
using SixNations.Server.Models;
using SixNations.Server.Services;

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
            var authUser = await _auth.AuthenticateAsync(username, password);
            var key = "access_token";
            // TODO move this into the AuthService
            var data = new Dictionary<string, object>
            {
                { "token_type", "Bearer" },
                { "expires_in", 86394 },
                { key, authUser.GetData()[key] }
            };
            var root = new ResponseRootObject(200, data);
            return Ok(root);
        }
    }
}