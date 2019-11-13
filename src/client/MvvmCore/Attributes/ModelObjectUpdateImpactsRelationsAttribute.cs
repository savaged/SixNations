using System;

namespace savaged.mvvm.Core.Attributes
{
    public class ModelObjectUpdateImpactsRelationsAttribute : Attribute
    {
        public ModelObjectUpdateImpactsRelationsAttribute()
        {
            Value = true;
        }

        public bool Value { get; }

        public static implicit operator bool(
            ModelObjectUpdateImpactsRelationsAttribute value)
        {
            if (value is null)
            {
                return false;
            }
            return value.Value;
        }

        public static explicit operator ModelObjectUpdateImpactsRelationsAttribute(
            bool value)
        {
            if (!value)
            {
                return null;
            }
            return new ModelObjectUpdateImpactsRelationsAttribute();
        }

    }

}