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
        where T : IHttpDataServiceModel
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
            var list = new List<T>();

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
                // TODO read headings and check for match with T fields

                // TODO read rows and map to T fields
            }
            wb?.Close(false, fi.Name, null);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(wbs);
            return list;
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
            if (heading)
            {
                value = kvp.Key;
            }
            ws.Cells[row, col] = value;
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