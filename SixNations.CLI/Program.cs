using System;
using System.Threading.Tasks;

namespace SixNations.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Kernel(args);
            Task t = app.RunAsync();
            t.GetAwaiter().GetResult();
        }
    }
}
