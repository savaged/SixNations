using System;

namespace savaged.mvvm.Navigation
{
    public class SelectedIndexChangedEventArgs : EventArgs
    {
        public SelectedIndexChangedEventArgs(
            int oldSelectedIndex, 
            int newSelectedIndex)
        {
            OldSelectedIndex = oldSelectedIndex;
            NewSelectedIndex = newSelectedIndex;
        }

        public int OldSelectedIndex { get; }

        public int NewSelectedIndex { get; }
    }
}
