using System;

namespace SixNations.CLI.IO
{
    enum Formats
    {
        Default,
        Success,
        Danger,
        Warning,
        Info
    }

    static class Feedback
    {
        internal static void Show(object value, Formats format = Formats.Default)
        {
            switch (format)
            {
                case Formats.Success:
                    Show(value, ConsoleColor.Yellow, ConsoleColor.Green);
                    break;
                case Formats.Danger:
                    Show(value, ConsoleColor.Yellow, ConsoleColor.Red);
                    break;                
                case Formats.Warning:
                    Show(value, ConsoleColor.Red, ConsoleColor.Yellow);
                    break;
                case Formats.Info:
                    Show(value, ConsoleColor.Black, ConsoleColor.Cyan);
                    break;
                case Formats.Default:
                    Console.WriteLine(value);
                    break;
            }
        }

        internal static void Splash()
        {
            Colorful.Console.WriteAscii("SixNations", System.Drawing.Color.Cyan);
        }

        private static void Show(
            object value, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}
