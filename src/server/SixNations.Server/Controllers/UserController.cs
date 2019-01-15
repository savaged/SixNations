using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixNations.Server.Data;
using SixNations.Server.Models;
using SixNations.Server.Services;
using System;
using System.Threading.Tasks;

namespace SixNations.Server.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
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

        // GET: User
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post([Bind("Firstname,Lastname,Password")] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            user.Password = _crypto.Encrypt(user.Password);
            user.Username = $"{user.Lastname}{user.Firstname}";

            try
            {
                _context.User.Add(user);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error adding model");
                throw;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null 
                    && ex.InnerException.Message.Contains("IX_User_Username"))
                {
                    ModelState.AddModelError(
                        "Username", "Correctly failed to add duplicate user - " +
                        "if users have the same name add a number to the firstname");
                    return BadRequest(ModelState);
                }
                else
                {
                    _logger.LogError(ex, "Failed to store new model in db");
                    throw;
                }
            }

            user.Password = null;

            return CreatedAtAction("Index", user);
        }
    }
}
