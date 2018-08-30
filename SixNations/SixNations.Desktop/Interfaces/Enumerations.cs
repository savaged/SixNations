using System;

namespace SixNations.Desktop.Interfaces
{
    public enum HttpMethods
    {
        Post,
        Get,
        Put,
        Delete,
        Patch
    }

    [Serializable]
    public enum ThemeOptions
    {
        Dark, 
        Light
    }
}
