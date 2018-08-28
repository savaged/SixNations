using System;
using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class StoryFilterMessage : MessageBase
    {
        public StoryFilterMessage(string filter)
        {
            Filter = filter;
        }

        public string Filter { get; }
    }
}
