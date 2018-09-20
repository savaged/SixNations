using System;

namespace SixNations.CLI.IO
{
    static class Entry
    {
        internal static string Read(string label, bool masked = false)
        {
            var value = string.Empty;
            Console.Write("{0}: ", label);
            if (masked)
            {
                value = GetMasked();
            }
            else
            {
                value = Console.ReadLine();
            }
            return value;
        }

        private static string GetMasked()
        {
            ConsoleKeyInfo key;
            var value = string.Empty;
            while (true)
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                if (key.Key != ConsoleKey.Backspace)
                {
                    value += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (value.Length > 0 && key.Key == ConsoleKey.Backspace)
                    {
                        value = value.Substring(0, (value.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            return value;
        }
    }
}
