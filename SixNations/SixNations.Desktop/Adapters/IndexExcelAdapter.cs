using System;
using log4net;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using SixNations.Desktop.Interfaces;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using SixNations.Desktop.Models;
using System.Threading.Tasks;

namespace SixNations.Desktop.Adapters
{
    public class IndexExcelAdapter<T> : IIndexExcelAdapter<T>
        where T : IHttpDataServiceModel, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Application _excel;
        private int _endCol;
        private int _endRow;

        public IndexExcelAdapter()
        {
            try
            {
                _excel = new Application();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Initiating Excel failed! {0}", ex.Message);
            }
            CanExecute = _excel != null;
        }

        ~IndexExcelAdapter()
        {
            if (_excel != null)
            {
                Workbooks wbs = _excel.Workbooks;
                foreach (Workbook wb in wbs)
                {
                    wb.Close(0);
                    Marshal.ReleaseComObject(wb);
                }
                Marshal.ReleaseComObject(wbs);
                _excel.Quit();
                Marshal.ReleaseComObject(_excel);
                _excel = null;
            }
        }

        public bool CanExecute { get; }

        public async Task<IList<T>> AdaptAsync(FileInfo fi)
        {
            var result = await Task.Run(() => Adapt(fi));
            return result;
        }

        public IList<T> Adapt(FileInfo fi)
        {
            Prechecks(fi);
            IList<T> index = null;

            var wbs = _excel.Workbooks;
            Workbook wb = null;
            try
            {
                wb = wbs.Open(fi.FullName);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Opening {0} failed! {1}", fi.Name, ex.Message);
                throw;
            }
            if (wb != null)
            {
                Sheets sheets = null;
                Worksheet ws = null;
                try
                {
                    sheets = wb.Worksheets;
                    ws = sheets[1];

                    var fields = FetchFields(ws);

                    index = FetchData(fields, ws);
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Unexpected error processing import! {0}", ex);
                }
                finally
                {
                    wb.Close(false, fi.Name, null);
                    Marshal.ReleaseComObject(wb);
                    Marshal.ReleaseComObject(ws);
                    Marshal.ReleaseComObject(sheets);
                }                
            }            
            Marshal.ReleaseComObject(wbs);
            return index;
        }

        public async Task AdaptAsync(IList<T> index)
        {
            await Task.Run(() => Adapt(index));
        }

        public void Adapt(IList<T> index)
        {
            if (index.Count == 0)
            {
                return;
            }
            Prechecks(index);

            Workbooks wbs = null;
            Workbook wb = null;
            Sheets sheets = null;
            Worksheet ws = null;
            try
            {
                wbs = _excel.Workbooks;
                wb = wbs[1];
                sheets = wb.Worksheets;
                ws = sheets[1];

                ApplyHeading(index, ws);

                var row = 2;
                foreach (var model in index)
                {
                    var col = 1;
                    var data = model.GetData();
                    foreach (var kvp in data)
                    {
                        ApplyValidCell(kvp, ws, row, ref col);
                    }
                    row++;
                }
                _endRow = row;

                FormatHeading(ws);
                _excel.Visible = true;
            }
            finally
            {
                Marshal.ReleaseComObject(ws);
                Marshal.ReleaseComObject(sheets);
                Marshal.ReleaseComObject(wb);
                Marshal.ReleaseComObject(wbs);
            }
        }

        private void Prechecks(IList<T> index)
        {
            if (index == null || index.FirstOrDefault() == null)
            {
                throw new ArgumentNullException("Expected a valid Index!");
            }
        }

        private void Prechecks(FileInfo fi)
        {
            if (!fi.Exists || !IsExcelExtension())
            {
                throw new ArgumentException("Expected a valid Excel file!");
            }
            bool IsExcelExtension()
            {
                string[] possibleExtensions = {
                    ".xls", ".xlsx", ".ods"
                };
                var matchedExtension = possibleExtensions
                    .Where(e => e == fi.Extension).FirstOrDefault();
                return !string.IsNullOrEmpty(matchedExtension);
            }
        }

        private IList<string> FetchFields(Worksheet ws)
        {
            var fields = new List<string>();
            var schema = new T().GetData();
            Range cells = null;
            try
            {
                cells = ws.Cells;
                for (var i = 1; i < schema.Count + 1; i++)
                {
                    Range cell = cells[1, i];
                    try
                    {
                        dynamic heading = cell.Value;
                        if (schema.ContainsKey(heading))
                        {
                            fields.Add(heading);
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(cell);
                    }
                }
            }
            finally
            {
                Marshal.ReleaseComObject(cells);
            }
            if (fields.Count != schema.Count)
            {
                throw new ArgumentException(
                    $"The worksheet headings do not match the fields for a {typeof(T).Name}!");
            }
            return fields;
        }

        private IList<T> FetchData(IList<string> fields, Worksheet ws)
        {
            var index = new List<T>();
            var data = new List<IDictionary<string, object>>();
            Range cells = null;
            try
            {
                cells = ws.Cells;
                var col = 1;
                var row = 2;
                while (true)
                {
                    if (!RowHasData(fields, ws, row))
                    {
                        Log.Debug("End of data to import.");
                        break;
                    }
                    Log.DebugFormat("Importing row {0}", row);
                    var item = new Dictionary<string, object>();
                    foreach (var field in fields)
                    {
                        Range cell = null;
                        try
                        {
                            Log.DebugFormat("\tImporting cell at column {0}", col);
                            cell = cells[row, col++];
                            dynamic value = cell?.Value;
                            if (value is double)
                            {
                                value = (int)value;
                            }
                            Log.DebugFormat("\t\tImporting value {0}:{1}", field, value);
                            item.Add(field, value);
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(cell);
                        }                        
                    }
                    data.Add(item);
                    row++;
                    col = 1;
                }
                var wrapper = new Dictionary<string, IList<IDictionary<string, object>>>
                {
                    { "Data", data }
                };
                var json = JsonConvert.SerializeObject(wrapper);
                var root = JsonConvert.DeserializeObject<ResponseRootObject>(
                    json, new ResponseConverter());
                root.Success = true;
                foreach (var dto in root?.Data)
                {
                    var model = new T();
                    model.Initialise(dto);
                    index.Add(model);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unexpected error converting data! {0}", ex);
                throw;
            }
            finally
            {
                Marshal.ReleaseComObject(cells);
            }            
            return index;
        }

        private bool RowHasData(IList<string> fields, Worksheet ws, int row)
        {
            var hasData = true;
            var joined = string.Empty;
            Range cells =null;
            try
            {
                cells = ws.Cells;
                var col = 1;
                foreach (var f in fields)
                {
                    joined = string.Empty;
                    Range content = cells[row, col++];
                    try
                    {
                        dynamic value = content?.Value;
                        joined += value?.ToString();
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(content);
                    }
                }
                hasData = joined.Length > 0;
            }
            finally
            {
                Marshal.ReleaseComObject(cells);
            }            
            return hasData;
        }

        private void ApplyHeading(IList<T> index, Worksheet ws)
        {
            var col = 1;
            var data = index.First().GetData();
            foreach (var kvp in data)
            {
                ApplyValidCell(kvp, ws, 1, ref col, true);
            }
            _endCol = col;
        }

        private void ApplyValidCell(
            KeyValuePair<string, object> kvp, Worksheet ws, int row, ref int col, bool heading = false)
        {
            var value = kvp.Value;
            Range cells = null;
            if (heading)
            {
                value = kvp.Key;
            }
            try
            {
                cells = ws.Cells;
                cells[row, col] = value;
                col++;
            }
            catch
            {
                Log.ErrorFormat(
                    "Possible data error with field [{0}] which has the " +
                    "value [{1}] trying to import to row [{2}] col [{3}]!",
                    kvp.Key, kvp.Value, row, col);
            }
            finally
            {
                Marshal.ReleaseComObject(cells);
            }
        }

        private void FormatHeading(Worksheet ws)
        {
            Range cell = null;
            Range row = null;
            Font font = null;
            try
            {
                cell = ws.Cells[1, 1];
                row = cell.EntireRow;
                font = row.Font;
                font.Bold = true;
            }
            finally
            {
                Marshal.ReleaseComObject(font);
                Marshal.ReleaseComObject(row);
                Marshal.ReleaseComObject(cell);
            }
        }
    }
}