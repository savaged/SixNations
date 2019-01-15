using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SixNations.Server.Data;
using SixNations.Server.Models;
using SixNations.Server.Services;
using System;
using System.Threading.Tasks;

namespace SixNations.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly IEncryptionService _crypto;

        public UserController(
            ApplicationDbContext context,
            ILogger<UserController> logger,
            IEncryptionService crypto)
        {
            _context = context;
            _logger = logger;
            _crypto = crypto;
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Password = _crypto.Encrypt(user.Password);

            _context.User.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store new model in db");
                throw;
            }

            user.Password = null;

            var root = new ResponseRootObject(201, user);
            return CreatedAtAction("GetUser", new { id = user.UserId }, root);
        }
    }
}
