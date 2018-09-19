using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;

namespace SixNations.CLI.Modules
{
    public class Login : BaseModule, ISubModule
    {
        public void Run()
        {
            var email = Entry.Read("Email");
            var password = Entry.Read("Password", true);
        }
    }
}
