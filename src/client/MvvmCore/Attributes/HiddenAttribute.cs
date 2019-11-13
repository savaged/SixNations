using System;

namespace savaged.mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HiddenAttribute : Attribute
    {
        public const string Value = "Not available to any ViewStateType";
    }
}
