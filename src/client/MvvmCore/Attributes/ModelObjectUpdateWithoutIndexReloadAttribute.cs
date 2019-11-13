using System;

namespace savaged.mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelObjectUpdateWithoutIndexReloadAttribute : Attribute
    {
        public ModelObjectUpdateWithoutIndexReloadAttribute()
        {
            Value = true;
        }

        public bool Value { get; }

        public static implicit operator bool(
            ModelObjectUpdateWithoutIndexReloadAttribute value)
        {
            if (value is null)
            {
                return false;
            }
            return value.Value;
        }

        public static explicit operator ModelObjectUpdateWithoutIndexReloadAttribute(
            bool value)
        {
            if (!value)
            {
                return null;
            }
            return new ModelObjectUpdateWithoutIndexReloadAttribute();
        }
    }
}
