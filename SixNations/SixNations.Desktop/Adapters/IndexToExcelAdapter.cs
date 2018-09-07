using System;
using log4net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Office.Interop.Excel;
using SixNations.Desktop.Interfaces;
using System.Runtime.InteropServices;

namespace SixNations.Desktop.Adapters
{
    public class IndexToExcelAdapter<T> : IIndexToExcelAdapter<T>
        where T : IHttpDataServiceModel
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Application _excel;
        private int _hiddenCols;
        private int _endCol;
        private int _endRow;

        public IndexToExcelAdapter()
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

        public bool CanExecute { get; }

        public void Adapt(IList<T> index)
        {
            if (index.Count == 0)
            {
                return;
            }
            Prechecks(index);

            var wbs = _excel.Workbooks;
            var wb = wbs.Add();
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
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(ws);
        }

        private void Prechecks(IList<T> index)
        {
            if (index == null || index.First() == null)
            {
                throw new ArgumentNullException("Expected a valid Index!");
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
            _endCol = col - _hiddenCols;
        }

        private void ApplyValidCell(
            KeyValuePair<string, object> kvp, Worksheet ws, int row, ref int col, bool heading = false)
        {
            var value = kvp.Value;
            if (heading)
            {
                value = kvp.Key;
            }
            if (!kvp.Key.ToLower().EndsWith("id"))
            {
                ws.Cells[row, col] = value;
                col++;
            }
            else
            {
                _hiddenCols++;
            }
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