﻿using System;
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
        internal static void Show(object value, Formats format = Formats.Default)
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
        }

        internal static void Show(IDataServiceModel model)
        {
            Console.WriteLine(model.ToJson());
        }

        internal static void Show(bool result)
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