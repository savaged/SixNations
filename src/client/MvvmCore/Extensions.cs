using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace savaged.mvvm.Core
{
    public static class Extensions
    {
        public static string SplitCamelCase(this string s)
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(s))
            {
                value = Regex.Replace(Regex.Replace(
                    s, 
                    @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            }
            return value;
        }

        public static string ToDecimalFormattedString(this string s)
        {
            var @try = decimal.TryParse(s, out decimal dec);
            if (@try)
            {
                s = string.Format("{0:n}", dec);
            }
            return s;
        }

        public static string FirstCharToUpper(this string s)
        {
            var value = string.Empty;
            if (!string.IsNullOrEmpty(s))
            {
                value = $"{s.First().ToString().ToUpper()}{s.Substring(1)}";
            }
            return value;
        }

        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
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

        public static string ToStringWithOrdinalPostfix(this int i)
        {
            if (i <= 0)
            {
                return i.ToString();
            }
            switch (i % 100)
            {
                case 11:
                case 12:
                case 13:
                    return $"{i}th";
            }
            switch (i % 10)
            {
                case 1:
                    return $"{i}st";
                case 2:
                    return $"{i}nd";
                case 3:
                    return $"{i}rd";
                default:
                    return $"{i}th";
            }
        }

        public static bool TryToDateTime(this string s, out DateTime result)
        {
            var @try = false;
            result = DateTime.MinValue;
            if (string.IsNullOrEmpty(s))
            {
                return @try;
            }
            string[] formats =
            {
                "dd/MM/yyyy",
                "dddd, dd MMMM yyyy",
                "dddd, dd MMMM yyyy HH:mm:ss",
                "MMMM dd",
                "yyyy MMMM",
                "dd.MM.yy",
                "d.M.yy",
                "dd-MM-yyyy",
                "yyyy-MM-dd",
                "yyyy'-'MM'-'dd",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss 'GMT'",
                "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                "yyyy-MM-dd HH:mm:ss",
                "dd-MM-yyyy HH:mm:ss",
                "HH:mm",
                "HH:mm tt",
                "H:mm tt",
                "HH:mm:ss"
            };
            @try = DateTime.TryParseExact(
                s, 
                formats, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, out DateTime date);
            if (@try || s == "0000-00-00")
            {
                result = date;
                @try = true;
            }
            return @try;
        }

        public static bool TryToNumber(this string s, out dynamic result)
        {
            var @try = false;
            if (string.IsNullOrEmpty(s))
            {
                result = s;
                return @try;
            }
            var isTelephoneNumber = false;
            if (s.TrimStart().StartsWith("0")
                && !s.StartsWith("0.")
                || s.TrimStart().StartsWith("+"))
            {
                isTelephoneNumber = true;
            }
            if (!isTelephoneNumber && s.Contains("."))
            {
                @try = decimal.TryParse(s, out decimal dec);
                if (@try)
                {
                    result = dec;
                    return @try;
                }
            }
            else if (!isTelephoneNumber)
            {
                @try = int.TryParse(s, out int @int);
                if (@try)
                {
                    result = @int;
                    return @try;
                }
            }
            result = s;
            return @try;
        }
    }
}
