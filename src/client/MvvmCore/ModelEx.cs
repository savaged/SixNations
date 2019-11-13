using savaged.mvvm.Core.Attributes;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace savaged.mvvm.Core
{
    public static class ModelEx
    {
        public static bool ModelObjectUpdateImpactsRelations(
            this IDataModel model)
        {
            var attribs = model.GetType().GetCustomAttributes(
                typeof(ModelObjectUpdateImpactsRelationsAttribute), true);

            bool isMatch = attribs.FirstOrDefault()
                is ModelObjectUpdateImpactsRelationsAttribute attrib
                ? attrib : false;

            return isMatch;
        }

        public static bool ModelObjectUpdateWithoutIndexReload(
            this IDataModel model)
        {
            var attribs = model.GetType().GetCustomAttributes(
                typeof(ModelObjectUpdateWithoutIndexReloadAttribute), true);

            bool isMatch = attribs.FirstOrDefault()
                is ModelObjectUpdateWithoutIndexReloadAttribute attrib
                ? attrib : false;

            return isMatch;
        }

        public static IObservableModel Clone(this IObservableModel source)
        {
            if (source == null) return source;

            var type = source.GetType();

            object destination = Activator.CreateInstance(type);

            var mc = new MapperConfiguration(c => c.CreateMap(type, type));
            var mapper = mc.CreateMapper();
            destination = mapper.Map(source, destination, type, type);

            var value = destination as IObservableModel;

            return value;
        }

        public static T Clone<T>(this T source)
            where T : IObservableModel, new()
        {
            if (source == null) return default;

            var value = new T();
            source.CopyTo(ref value);
            return value;
        }

        public static bool IsNullOrNew(this IDataModel model)
        {
            if (model != null)
            {
                return model.IsNew;
            }
            return true;
        }

        public static void SetProperty(
            this IObservableModel model, object input, PropertyInfo p)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (p is null)
            {
                throw new ArgumentNullException(nameof(p));
            }
            //var value = Convert.ChangeType(input, p.PropertyType);
            var propType = p.PropertyType;
            var fieldType = input.GetType();
            dynamic value = null;
            if (propType == fieldType)
            {
                if (fieldType == typeof(string))
                {
                    input = input.ToString().Trim();
                }
                value = input;
            }
            else if (propType == typeof(decimal) && fieldType.IsNumeric())
            {
                value = decimal.Parse(input.ToString());
            }
            else if (propType == typeof(decimal) && fieldType == typeof(string))
            {
                var result = decimal.TryParse(input.ToString(), out decimal dec);
                if (result)
                {
                    value = dec;
                }
            }
            else if (propType == typeof(string))
            {
                value = input.ToString().Replace(" 00:00:00", string.Empty);
            }
            else if (propType == typeof(DateTime))
            {
                var @try = input.ToString().TryToDateTime(out DateTime date);
                if (@try)
                {
                    value = date;
                }
                else
                {
                    value = DateTime.MinValue;
                }
            }
            else if (propType == typeof(DateTime?))
            {
                var @try = input.ToString().TryToDateTime(out DateTime date);
                if (@try)
                {
                    value = date;
                }
                else
                {
                    value = (DateTime?)null;
                }
            }
            else if (propType == typeof(bool))
            {
                var @try = bool.TryParse(input.ToString(), out bool boolean);
                if (@try)
                {
                    value = boolean;
                }
            }
            else if (propType == typeof(int))
            {
                var @try = int.TryParse(input.ToString(), out int i);
                if (@try)
                {
                    value = i;
                }
            }
            try
            {
                if (p.CanWrite)
                {
                    p.SetValue(model, value);
                }
                else
                {
                    //TrySetReadOnlyPropertyValue(p, value);
                    var s = p.DeclaringType.GetProperty(p.Name).GetSetMethod(true);
                    if (s != null)
                    {
                        s.Invoke(model, new object[] { value });
                    }
                }
            }
            catch (InvalidCastException ice)
            {
                throw new DesktopException(
                    "Data type mismatch error on input field with type: " +
                    $"[{fieldType}] for model property: [{p.Name}] with" +
                    $"type: [{propType}]."
                    , ice);
            }
        }

    }
}
