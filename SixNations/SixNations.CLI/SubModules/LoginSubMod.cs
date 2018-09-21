using System;
using System.Threading.Tasks;
using SixNations.API.Exceptions;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.Modules;
using SixNations.Data.Models;

namespace SixNations.CLI.SubModules
{
    public class LoginSubMod : BaseModule, ISubModule
    {
        private readonly IAuthTokenService _authTokenService;

        public LoginSubMod(
            IInputEntryService entryService,
            IAuthTokenService authTokenService)
            : base(entryService)
        {
            _authTokenService = authTokenService;
        }

        public async Task RunAsync()
        {
            var email = Entry.Read("Email");
            var password = Entry.Read("Password", true);
            var token = string.Empty;
            try
            {
                token = await _authTokenService.GetTokenAsync(email, password);
            }
            catch (AuthServiceException ex)
            {
                Feedback.Show(ex, Formats.Danger);
            }
            if (!string.IsNullOrEmpty(token))
            {
                User.Current.Initialise(token);
            }
        }
    }
}
