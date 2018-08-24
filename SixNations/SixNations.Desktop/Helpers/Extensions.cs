using System;
using System.Text.RegularExpressions;

namespace SixNations.Desktop.Helpers
{
    public static class Extensions
    {
        public static string NameToUriFormat(this Type t)
        {
            return Regex.Replace(t.Name, @"`[\d-]", string.Empty).ToLower();
        }

        public static bool IsNumeric(this Type t)
        {
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
