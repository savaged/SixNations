using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> PostLogin()
        {
            // TODO set-up and integrate a standard identity server like identityserver4 or
            // something like the ASP.Net Core auth demo:
            // https://github.com/aspnet/Docs/tree/master/aspnetcore/security/authentication/identity/sample/src/ASPNETCore-IdentityDemoComplete/IdentityDemo

            await Task.CompletedTask;
            return Ok("todo");
        }
    }
}