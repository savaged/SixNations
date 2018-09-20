using System;
using System.Text.RegularExpressions;

namespace SixNations.API.Helpers
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

        public static bool IsNumeric(this object value)
        {
            if (value is byte
                || value is sbyte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong
                || value is float
                || value is double
                || value is decimal)
            {
                return true;
            }
            else if (value is string s)
            {
                return s.IsNumeric();
            }
            return false;
        }

        public static bool IsNumeric(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (byte.TryParse(value, out byte b)
                    || sbyte.TryParse(value, out sbyte sb)
                    || short.TryParse(value, out short s)
                    || ushort.TryParse(value, out ushort us)
                    || int.TryParse(value, out int i)
                    || uint.TryParse(value, out uint ui)
                    || long.TryParse(value, out long l)
                    || ulong.TryParse(value, out ulong ul)
                    || float.TryParse(value, out float f)
                    || double.TryParse(value, out double d)
                    || decimal.TryParse(value, out decimal n))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
    