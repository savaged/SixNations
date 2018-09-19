using System;
using System.Threading.Tasks;

namespace SixNations.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Kernel(args);
            app.RunAsync().GetAwaiter().GetResult();
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}
