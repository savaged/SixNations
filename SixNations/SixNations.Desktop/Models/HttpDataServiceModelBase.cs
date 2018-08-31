using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using GalaSoft.MvvmLight;
using SixNations.Desktop.Attributes;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Models
{
    public abstract class HttpDataServiceModelBase 
        : ObservableObject, IHttpDataServiceModel
    {
        private bool _isDirty;
        private bool _isInitialised;
        
        public virtual void Initialise(DataTransferObject dto)
        {
            if (_isInitialised)
            {
                throw new NotSupportedException(
                    "Trying to reinitialise which is not supported!");
            }
            var type = GetType();
            var props = type.GetProperties();
            foreach (var p in props)
            {
                SetProperty(dto, p);
            }
            PropertyChanged += OnPropertyChanged;
            _isInitialised = true;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsDirty):
                case nameof(IsLockedForEditing):
                    break;
                default:
                    IsDirty = true;
                    break;
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

        [Hidden]
        public string Error { get; set; }

        [Hidden]
        public bool IsLockedForEditing { get; private set; }

        [Hidden]
        public bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }

        public abstract int Id { get; }

        [Hidden]
        public bool IsNew => Id < 1;

        [Hidden]
        public bool IsReadOnly => !IsLockedForEditing && !IsNew;

        public IDictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            var props = GetType().GetProperties();
            foreach (var p in props)
            {
                // Transforms using attributes can be added here

                if (!Attribute.IsDefined(p, typeof(HiddenAttribute)))
                {
                    var value = p.GetValue(this);
                    if (value is DateTime)
                    {
                        value = FilterAnyMinDate(value);
                    }
                    data.Add(p.Name, value);
                }
            }
            return data;
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
