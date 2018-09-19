using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using SixNations.CLI.Interfaces;
using SixNations.CLI.Modules;

namespace SixNations.CLI
{
    static class Router
    {
        internal static IEnumerable<IModule> Route(string[] args)
        {
            var all = SimpleIoc.Default.GetAllInstances<IModule>();
            var selected = new List<IModule>
            {
                all.Where(m => m.Name == nameof(Main)).First()
            };
            foreach (var arg in args)
            {
                var module = all.Where(m => m.Name.ToLower() == arg).FirstOrDefault();
                if (module != null)
                {
                    selected.Add(module);
                }
                switch (arg.ToLower())
                {
                    case "version":
                    case "ver":
                    case "--version":
                    case "-version":
                    case "--v":
                    case "-v":
                        selected.Add(all.Where(m => m.Name == nameof(About)).First());
                        break;
                    case "?":
                    case "-h":
                    case "--h":
                    case "--help":
                    case "-help":
                        selected.Add(all.Where(m => m.Name == nameof(Help)).First());
                        break;
                }
            }
            return selected;
        }
    }
}
