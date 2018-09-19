using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.Data.Models;

namespace SixNations.CLI.Modules
{
    public class Main : BaseModule, IModule
    {
        public void Run()
        {
            throw new NotSupportedException();
        }

        public async Task RunAsync()
        {
            await SimpleIoc.Default.GetInstance<Login>().RunAsync();
            if (!User.Current.IsLoggedIn)
            {
                Feedback.Show("Login Failure!", Formats.Danger);
                return;
            }
            Feedback.Show("Login Success!", Formats.Success);
            // TODO more logic here ;)
        }
    }
}
