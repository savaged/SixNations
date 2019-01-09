using System;
using System.Drawing;
using SixNations.API.Interfaces;

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
        internal static void Show(
            object value, Formats format = Formats.Default, bool lineBreakAfter = false)
        {
            switch (format)
            {
                case Formats.Success:
                    Show(value, ConsoleColor.Blue, ConsoleColor.Green);
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
            if (lineBreakAfter)
            {
                Console.WriteLine();
            }
        }

        internal static void Show(IDataServiceModel model, bool lineBreakAfter = false)
        {
            Console.WriteLine(model.ToJson());
            if (lineBreakAfter)
            {
                Console.WriteLine();
            }
        }

        internal static void Show(bool result, bool lineBreakAfter = false)
        {
            Color foreground;
            if (result)
            {
                foreground = Color.Green;
            }
            else
            {
                foreground = Color.Red;
            }
            Colorful.Console.WriteLine("Success!", foreground);
            if (lineBreakAfter)
            {
                Console.WriteLine();
            }
        }

        internal static void Splash()
        {
            Colorful.Console.WriteAscii("SixNations", Color.Cyan);
        }

        internal static void ShowMenu()
        {
            
        }

        private static void Show(
            object value, ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.WriteLine(value);
            Console.ResetColor();
        }

        private static void WriteLineAtBottom()
        {

        }
    }
}
