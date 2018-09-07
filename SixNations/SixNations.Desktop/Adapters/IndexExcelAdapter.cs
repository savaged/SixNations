using System;
using log4net;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using SixNations.Desktop.Interfaces;
using System.Runtime.InteropServices;

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
                _excel.Quit();
                Marshal.ReleaseComObject(_excel);
            }
        }

        public bool CanExecute { get; }

        public IList<T> Adapt(FileInfo fi)
        {
            Prechecks(fi);
            var index = new List<T>();

            var wbs = _excel.Workbooks;
            Workbook wb = null;
            try
            {
                wb = wbs.Open(fi.Name);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Opening {0} failed! {1}", fi.Name, ex.Message);
            }
            if (wb != null)
            {
                var sheets = wb.Worksheets;
                var ws = sheets[1];

                var fields = FetchFields(ws);

                index = FetchData(fields, ws);

                Marshal.ReleaseComObject(ws);
                Marshal.ReleaseComObject(sheets);
            }
            wb?.Close(false, fi.Name, null);

            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(wbs);
            return index;
        }

        public void Adapt(IList<T> index)
        {
            if (index.Count == 0)
            {
                return;
            }
            Prechecks(index);

            Workbooks wbs = _excel.Workbooks;
            var wb = wbs[1];
            var sheets = wb.Worksheets;
            var ws = sheets[1];

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

            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(wbs);
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
                    "xls", "xlsx", "ods"
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
            var cells = ws.Cells;
            for (var i = 1; i < schema.Count; i++)
            {
                var heading = cells[1, i++];
                if (schema.ContainsKey(heading))
                {
                    fields.Add(heading);
                }
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
            var cells = ws.Cells;
            var data = new Dictionary<string, object>();
            var col = 1;
            var count = 0;
            var index = new List<T>();
            foreach (var field in fields)
            {
                var row = 2;
                var column = new Dictionary<string, object>();
                while (true)
                {                                        
                    if (!RowHasData(fields, ws, row))
                    {
                        count = row - 1;
                        break;
                    }
                    var value = cells[row++, col];
                    column.Add(field, value);
                    // TODO map the model
                }
                // ?? columns.Add(field, column);
                col++;
            }
            return index;
        }

        private bool RowHasData(IList<string> fields, Worksheet ws, int row)
        {
            var cells = ws.Cells;
            var hasData = true;
            var col = 1;
            foreach (var f in fields)
            {
                var content = cells[row, col++];
                hasData &= !string.IsNullOrEmpty(content);
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
            var cells = ws.Cells;
            if (heading)
            {
                value = kvp.Key;
            }
            try
            {
                cells[row, col] = value;
            }
            catch
            {
                Log.ErrorFormat(
                    "Possible data error with field [{0}] which has the " +
                    "value [{1}] trying to import to row [{2}] col [{3}]!",
                    kvp.Key, kvp.Value, row, col);
            }
            col++;
        }

        private void FormatHeading(Worksheet ws)
        {
            var cells = ws.Cells[1, 1];
            var row = cells.EntireRow;
            var font = row.Font;
            font.Bold = true;
        }
    }
}