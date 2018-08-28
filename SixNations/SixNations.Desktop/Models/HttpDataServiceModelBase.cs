using System;
using System.Collections.Generic;
using System.Reflection;
using GalaSoft.MvvmLight;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Models
{
    public abstract class HttpDataServiceModelBase : ObservableObject, IHttpDataServiceModel
    {
        private bool _isDirty;

        public virtual void Initialise(DataTransferObject dto)
        {
            var type = GetType();
            var props = type.GetProperties();
            foreach (var p in props)
            {
                SetProperty(dto, p);
            }
        }

        private void SetProperty(DataTransferObject dto, PropertyInfo p)
        {
            var key = p.Name;
            if (!dto.ContainsKey(key))
            {
                return;
            }
            if (dto[key] == null)
            {
                return;
            }
            var propType = p.PropertyType;
            var fieldType = dto[key].GetType();

            dynamic value = null;
            if (propType == fieldType)
            {
                value = dto[key];
            }
            else if (propType == typeof(decimal) && fieldType.IsNumeric())
            {
                value = decimal.Parse(dto[key].ToString());
            }
            else if (propType == typeof(string))
            {
                value = dto[key].ToString().Replace(" 00:00:00", string.Empty);
            }
            try
            {
                if (p.CanWrite)
                {
                    p.SetValue(this, value);
                }
                else
                {
                    TrySetReadOnlyPropertyValue(p, value);
                }
            }
            catch (InvalidCastException ex)
            {
                throw new Exception(
                    "Data type mismatch error on model key: " +
                    $"{key} for property: {p.Name}", ex);
            }
        }

        private void TrySetReadOnlyPropertyValue(PropertyInfo p, dynamic value)
        {
            var s = p.DeclaringType.GetProperty(p.Name).GetSetMethod(true);
            if (s != null) s.Invoke(this, new object[] { value });
        }

        public string Error { get; set; }

        public bool IsLockedForEditing { get; private set; }

        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }

        public abstract int Id { get; }

        public bool IsNew => Id < 1;

        public bool IsReadOnly => !IsLockedForEditing && !IsNew;

        public IDictionary<string, object> Data
        {
            get
            {
                var data = new Dictionary<string, object>();
                var props = GetType().GetProperties();
                foreach (var p in props)
                {
                    if (p.Name == nameof(Data)) continue;

                    // Transforms using attributes can be added here

                    var value = p.GetValue(this);
                    if (value is DateTime)
                    {
                        value = FilterAnyMinDate(value);
                    }
                    data.Add(p.Name, value);
                }
                return data;
            }
        }

        private static object FilterAnyMinDate(object value)
        {
            if (value is DateTime dt)
            {
                if (dt == DateTime.MinValue)
                {
                    return null;
                }
            }
            return value;
        }

    }
}
