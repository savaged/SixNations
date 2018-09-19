using System;

namespace SixNations.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new Kernel(args);
            app.Run();
        }
    }
}
