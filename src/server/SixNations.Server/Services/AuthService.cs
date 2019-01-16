using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SixNations.Server.Data;
using SixNations.Server.Models;

namespace SixNations.Server.Services
{
    /// <summary>
    /// Very simple authentification, however
    /// this could be swapped out in favour of 
    /// ASP.Net Core's authentification.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _crypto;

        public AuthService(ApplicationDbContext context, IEncryptionService crypto)
        {
            _context = context;
            _crypto = crypto;
        }

        public async Task<Token> AuthenticateAsync(
            string username, string password)
        {
            Token value = null;

            var index = await _context.User.ToListAsync();

            password = _crypto.Encrypt(password);

            var user = index
                .Where(u => u.Username == username)
                .Where(u => u.Password == password)
                .FirstOrDefault();
            
            if (user != null)
            {
                //authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Props.ClientSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var access_token = tokenHandler.WriteToken(token);
                var expires_in = (tokenDescriptor.Expires - DateTime.Now).Value.TotalSeconds;
                value = new Token("Bearer", (int)expires_in, access_token);
            }
            return value;
        }
    }
}
