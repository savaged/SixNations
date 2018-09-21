using System;
using System.Threading.Tasks;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.SubModules;
using SixNations.Data.Models;

namespace SixNations.CLI.Modules
{
    public class MainMod : BaseModule, IModule
    {
        private LoginSubMod _loginSubMod;
        private RequirementsSubMod _indexSubMod;

        public MainMod(LoginSubMod loginSubMod, RequirementsSubMod indexSubMod)
        {
            _loginSubMod = loginSubMod;
            _indexSubMod = indexSubMod;
        }

        public async Task RunAsync()
        {
            await _loginSubMod.RunAsync();
            if (!User.Current.IsLoggedIn)
            {
                Feedback.Show("Login Failure!", Formats.Danger);
                return;
            }
            Feedback.Show("Login Success!", Formats.Success);

            await _indexSubMod.RunAsync();
        }
    }
}
