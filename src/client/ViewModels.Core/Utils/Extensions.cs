using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public static class Extensions
    {
        public static T ToModel<T>(this DataRow dr)
            where T : IObservableModel, new()
        {
            var model = new T();
            foreach (DataColumn col in dr.Table.Columns)
            {
                var colName = col.ColumnName?.Trim();

                var p = model.GetType().GetProperty(colName);

                if (p != null && dr[col] != DBNull.Value)
                {
                    model.SetProperty(dr[col], p);
                }
            }
            return model;
        }

        public static string ToValidFileName(this string fileName)
        {
            fileName = string.Join(
                string.Empty, 
                fileName.Split(Path.GetInvalidFileNameChars()));

            var apiInvalidChars = "£$€'".ToCharArray();
            fileName = string.Join(
                string.Empty,
                fileName.Split(apiInvalidChars));

            return fileName;
        }

        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, 
            KeyValuePair<TKey, TValue> kvp)
        {
            if (dict.ContainsKey(kvp.Key))
            {
                dict[kvp.Key] = kvp.Value;
            }
            else
            {
                dict.Add(kvp);
            }
            return dict;
        }

    }
}
