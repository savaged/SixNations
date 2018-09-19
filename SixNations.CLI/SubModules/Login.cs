using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using SixNations.API.Exceptions;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class Login : BaseModule, ISubModule
    {
        public void Run()
        {
            throw new NotSupportedException();
        }

        public async Task RunAsync()
        {
            var email = Entry.Read("Email");
            var password = Entry.Read("Password", true);
            var token = string.Empty;
            try
            {
                token = await SimpleIoc.Default.GetInstance<IAuthTokenService>()
                    .GetTokenAsync(email, password);
            }
            catch (AuthServiceException ex)
            {
                // TODO feedback
            }
        }
    }
}
