using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace savaged.mvvm.Data
{
    public static class Extensions
    {
        public static T ChangeType<T>(this object obj)
        {
            T value = default;
            if (obj != null)
            {
                try
                {
                    value = (T)Convert.ChangeType(obj, typeof(T));
                }
                catch (Exception ex)
                {
                    throw new ApiDataException(
                        $"Converting the API data '{obj}' to the " +
                        $"expected {typeof(T).Name} type failed!", ex);
                }
            }
            return value;
        }

        public static IDictionary<string, object> ToDictionary(
            this object o)
        {
            var j = JsonConvert.SerializeObject(o);
            var value = JsonConvert
                .DeserializeObject<IDictionary<string, object>>(j);
            return value;
        }

        public static string ToUriParams(
            this IDictionary<string, object> dict)
        {
            var value = string.Format("?{0}", string.Join("&", dict
                .Select(kvp => string.Format(
                    "{0}={1}", kvp.Key, kvp.Value)
                )));
            return value;
        }

        public static string TrimIfQuoted(this string s, char quote)
        {
            if (s.StartsWith(quote.ToString())
                && s.EndsWith(quote.ToString()))
            {
                s = s.Remove(0, 1);
                s = s.Remove(s.Length - 1);
            }
            return s;
        }

        public static string ToUriFormat(this string s)
        {
            return Regex.Replace(s, @"`[\d-]", string.Empty).ToLower();
        }

    }
}
