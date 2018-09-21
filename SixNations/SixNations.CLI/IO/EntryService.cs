using System;

namespace SixNations.CLI.IO
{
    public class EntryService : IInputEntryService
    {
        private const string _prompt = ": ";

        public event EventHandler<EventArgs> Escaped;

        public string Read(string label, bool masked = false)
        {
            var value = string.Empty;
            Console.Write("{0}{1}", label, _prompt);
            value = Read(masked);
            return RemoveLabel(label, value);
        }

        private string Read(bool masked = false)
        {
            ConsoleKeyInfo key;
            var value = string.Empty;
            while (true)
            {
                key = ReadOrComplete(out bool isReadingCompleted);
                if (isReadingCompleted)
                {
                    break;
                }
                if (key.Key != ConsoleKey.Backspace)
                {
                    value += key.KeyChar;
                    if (masked)
                    {
                        Console.Write("*");
                    }
                    else
                    {
                        Console.Write(key.KeyChar);
                    }
                }
                else if (value.Length > 0 && key.Key == ConsoleKey.Backspace)
                {
                    value = value.Substring(0, (value.Length - 1));
                    Console.Write("\b \b");
                }
            }
            return value;
        }

        private ConsoleKeyInfo ReadOrComplete(out bool isReadingCompleted)
        {
            isReadingCompleted = false;
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    RaiseEscaped();
                    isReadingCompleted = true;
                    break;
                case ConsoleKey.Enter:
                    Console.WriteLine();
                    isReadingCompleted = true;
                    break;
            }
            return key;
        }

        private string RemoveLabel(string label, string entry)
        {
            var value = entry.Replace($"{label}{_prompt}", string.Empty);
            return value;
        }

        private void RaiseEscaped()
        {
            var handler = Escaped;
            handler?.Invoke(this, new EventArgs());
        }
    }
}
