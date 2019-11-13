using System;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using log4net;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using savaged.mvvm.Core.Attributes;
using savaged.mvvm.Core.Interfaces;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class IndexToExcelAdapter<T> : IIndexAdapter
        where T : IObservableModel
    {
        private static readonly ILog _log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IList<T> _index;
        private Application _excel;
        private int _hiddenCols;
        private int _endCol;
        private int _endRow;

        public IndexToExcelAdapter(IList<T> index)
        {
            _index = index ?? throw new ArgumentNullException(
                "There must always be an index");
            _excel = new Application() ?? throw new ArgumentNullException(
                "Excel is not properly installed");
            _excel.SheetsInNewWorkbook = 1;
            _log.Info("Index to Excel Adapter ready.");
        }

        ~IndexToExcelAdapter()
        {
            _log.Info("Index to Excel Adapter disposing.");
            if (_excel != null)
            {
                Marshal.ReleaseComObject(_excel);
            }
        }


        public bool Adapt()
        {
            _log.Info("Index to Excel Adapter started.");
            if (_index.Count == 0)
            {
                _log.Warn("Index to Excel Adapter has found the index is empty!");
            }
            _log.Info("Index to Excel Adapter running pre-checks.");
            Prechecks();

            _log.Info("Index to Excel Adapter creating Excel Workbook structure.");
            Workbooks wbs = null;
            Workbook wb = null;
            Sheets sheets = null;
            Worksheet ws = null;
            try
            {
                wbs = _excel.Workbooks;
                wb = wbs.Add(XlWBATemplate.xlWBATWorksheet);
                sheets = wb.Worksheets;
                ws = (Worksheet)sheets[1];
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error creating Excel workbook structure!\n\r{0}", ex.Message);
            }
            _log.Info("Index to Excel Adapter checking Excel workbook structure.");
            StructureChecks(wbs, wb, sheets, ws);
            _log.Info("Index to Excel Adapter creating headings from index.");
            try
            {
                ApplyHeading(ws);
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error creating headings from index!\n\r{0}", ex.Message);
            }
            _log.Info("Index to Excel Adapter adding data from index.");
            try
            {
                var row = 2;
                foreach (var model in _index)
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
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error adding data from index!\n\r{0}", ex.Message);
            }
            _log.Info("Index to Excel Adapter formating headings.");
            try
            {
                FormatHeading(ws);
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error formatting headings!\n\r{0}", ex.Message);
            }
            _log.Info("Index to Excel Adapter saving Excel Workbook to temp file.");
            var tmpFile = Save(wb);
            _log.InfoFormat("Index to Excel Adapter saved Excel Workbook to temp file at {0} and closing workbook.", tmpFile);
            wb.Close();
            Marshal.ReleaseComObject(ws);
            Marshal.ReleaseComObject(sheets);
            Marshal.ReleaseComObject(wb);
            Marshal.ReleaseComObject(wbs);
            _log.Info("Index to Excel Adapter opening saved Workbook in Excel.");
            try
            {
                Process.Start(tmpFile);
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error opening temp workbook: {0}!\n\r{1}", tmpFile, ex.Message);
            }
            _log.Info("Index to Excel Adapter opened saved Workbook in Excel.");
            return true;
        }

        private string Save(Workbook wb)
        {
            var tmpFile = $"{Path.GetTempPath()}{Path.GetRandomFileName()}.xlsx";
            try
            {
                wb.SaveAs(tmpFile, XlFileFormat.xlWorkbookDefault);
            }
            catch (Exception ex)
            {
                // TODO Important! Catch the specific exception type(s) only
                _log.ErrorFormat("Error saving temp workbook: {0}!\n\r{1}", tmpFile, ex.Message);
            }
            return tmpFile;
        }

        private void Prechecks()
        {
            if (_index is null)
            {
                throw new ArgumentNullException("Expected a valid Index!");
            }
        }

        private void StructureChecks(Workbooks wbs, Workbook wb, Sheets sheets, Worksheet ws)
        {
            if (wbs == null) throw new ArgumentNullException("Workbooks");
            if (wb == null) throw new ArgumentNullException("Workbook");
            if (sheets == null) throw new ArgumentNullException("Sheets");
            if (ws == null) throw new ArgumentNullException("Worksheet");
        }

        private void ApplyHeading(Worksheet ws)
        {
            var col = 1;
            var model = _index.FirstOrDefault();
            IDictionary<string, object> data = new Dictionary<string, object>();
            if (model != null)
            {
                data = model.GetData();
            }
            else
            {
                var type = typeof(T);
                var props = type.GetProperties();
                foreach (var p in props)
                {
                    if (!Attribute.IsDefined(p, typeof(HiddenAttribute)))
                    {
                        data.Add(p.Name, null);
                    }
                }
            }
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
                try
                {
                    ws.Cells[row, col] = value;
                }
                catch
                {
                    _log.ErrorFormat(
                        "Possible data error with field [{0}] which has the " +
                        "value [{1}] trying to import to row [{2}] col [{3}]!", 
                        kvp.Key, kvp.Value, row, col);
                }
                col++;
            }
            else
            {
                _hiddenCols++;
            }
        }

        private void FormatHeading(Worksheet ws)
        {
            var cells = (Range)ws.Cells[1, 1];
            var row = cells.EntireRow;
            var font = row.Font;
            font.Bold = true;
        }
    }
}
