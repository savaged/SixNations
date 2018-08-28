using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Models
{
    public class Lookup : Dictionary<int, string>, IHttpDataServiceModel
    {
        public Lookup(string lookupName, DataTransferObject[] data)
        {
            Name = lookupName;
            if (data == null)
            {
                throw new ArgumentNullException("Expected to recieve initialised data.");
            }
            foreach (var dto in data)
            {
                Initialise(dto);
            }
        }

        public Lookup(string lookupName, ResponseRootObject response) 
            : this(lookupName, response?.Data) { }

        public Lookup()
        {
            Add(0, "Unknown");
        }

        public Lookup(string @default)
        {
            Add(0, @default);
        }

        public Lookup(int[] array)
        {
            foreach (var value in array)
            {
                Add(value, value.ToString());
            }
        }

        public Lookup(IDictionary<int, string> dict)
        {
            foreach (var kvp in dict)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public Lookup(Enum @enum)
        {
            foreach (var field in Enum.GetValues(@enum.GetType()))
            {
                Add((int)field, field.ToString());
            }
        }

        public void Initialise(DataTransferObject dto)
        {
            if (dto.ContainsKey($"{Name}ID") || dto.ContainsKey("ID"))
            {
                var key = dto.ContainsKey($"{Name}ID") ?
                    (int)dto[$"{Name}ID"] : (int)dto["ID"];
                var value = dto.ContainsKey($"{Name}Name") ?
                    dto[$"{Name}Name"].ToString() : dto["Name"].ToString();
                Add(key, value);
            }
        }

        public string Name { get; }

        public string Error { get; set; }

        public bool IsLockedForEditing => false;

        public bool IsDirty => false;

        public int Id => throw new NotSupportedException();

        public bool IsNew => false;

        public bool IsReadOnly => true;

        public IDictionary<string, object> Data => throw new NotSupportedException();
    }
}