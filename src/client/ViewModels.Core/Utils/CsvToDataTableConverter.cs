using CsvHelper;
using System.Data;
using System.IO;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public static class CsvToDataTableConverter
    {
        public static DataTable GetDataTableFromCsv(string csv)
        {
            var value = new DataTable();
            if (string.IsNullOrEmpty(csv))
            {
                return value;
            }
            using (var textReader = new StringReader(csv))
            {
                using (var csvReader = new CsvReader(textReader))
                {
                    using (var csvDataReader = new CsvDataReader(csvReader))
                    {
                        value.Load(csvDataReader);
                    }
                }
            }
            return value;
        }
    }
}
