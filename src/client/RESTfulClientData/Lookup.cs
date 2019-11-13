using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace savaged.mvvm.Data
{
    public class Lookup : ILookup
    {
        public Lookup(string @default)
            : this()
        {
            Add(0, @default);
        }

        public Lookup(int[] values)
            : this()
        {
            foreach (var value in values)
            {
                Add(value, value.ToString());
            }
        }

        public Lookup(IDictionary<int, string> dict)
            : this()
        {
            foreach (var kvp in dict)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        public Lookup(Enum @enum)
            : this()
        {
            foreach (var field in Enum.GetValues(@enum.GetType()))
            {
                Add((int)field, field.ToString());
            }
        }

        public Lookup(Exception ex, string suggestion = "")
            : this($"Exception: {ex.Message} Suggestion: {suggestion}") { }

        public Lookup()
        {
            Dictionary = new Dictionary<int, string>();
        }

        private IDictionary<int, string> Dictionary { get; }

        public int GetKeyFromValue(string value)
        {
            var match = 0;
            if (!string.IsNullOrEmpty(value))
            {
                var kvp = this.Where(
                            i => i.Value.ToLower() == value.ToLower())
                            .FirstOrDefault();
                match = kvp.Key;
            }
            return match;
        }

        public ILookup ToOrdered(bool descending = false)
        {
            IOrderedEnumerable<KeyValuePair<int, string>> ordered;
            if (descending)
            {
                ordered = this.OrderByDescending(l => l.Value);
            }
            else
            {
                ordered = this.OrderBy(l => l.Value);
            }
            var dict = ordered.ToDictionary(p => p.Key, p => p.Value);
            var value = new Lookup(dict);
            return value;
        }


        public string this[int key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        public ICollection<int> Keys => Dictionary.Keys;

        public ICollection<string> Values => Dictionary.Values;

        public int Count => Dictionary.Count;

        public bool IsReadOnly => Dictionary.IsReadOnly;

        public void Add(int key, string value)
        {
            Dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<int, string> item)
        {
            Dictionary.Add(item);
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<int, string> item)
        {
            return Dictionary.Contains(item);
        }

        public bool ContainsKey(int key)
        {
            return Dictionary.ContainsKey(key);
        }

        public void CopyTo(
            KeyValuePair<int, string>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        public bool Remove(int key)
        {
            return Dictionary.Remove(key);
        }

        public bool Remove(KeyValuePair<int, string> item)
        {
            return Dictionary.Remove(item);
        }

        public bool TryGetValue(int key, out string value)
        {
            return Dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
    }
}
