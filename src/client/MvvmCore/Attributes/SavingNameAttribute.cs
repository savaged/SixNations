using System;

namespace savaged.mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SavingNameAttribute : Attribute
    {
        public SavingNameAttribute(string value)
        {
            SavingName = value;
        }

        public SavingNameAttribute(bool variableParent)
        {
            VariableParent = variableParent;
            SavingName = "ParentId";
        }

        public readonly string SavingName;

        public readonly bool VariableParent;
    }
}
