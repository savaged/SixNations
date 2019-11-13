using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MSOutlook = Microsoft.Office.Interop.Outlook;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public static class DragDropHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        public static string DraggedItemSourceName(DragEventArgs e)
        {
            var value = (e.Source as Button)?.Name;
            return value;
        }

        public static IModelWithUploads DraggedItemToModelWithUploads(
            DragEventArgs e)
        {
            var arg = (e.Source as Button)?.CommandParameter;
            if (arg == null)
            {
                throw new ArgumentNullException("CommandParameter");
            }
            var value = (IModelWithUploads)arg;
            return value;
        }


        public static string DraggedItemToUserFileLocation(
            DragEventArgs e)
        {
            string filename = string.Empty;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // File(s) from Windows Explorer
                string[] files = (string[])e.Data.GetData(
                    DataFormats.FileDrop);
                // Just taking the first
                filename = files[0];
            }
            else if (e.Data.GetDataPresent("FileGroupDescriptor"))
            {
                var selectedFileName = 
                    GetFileGroupDescriptorFileName(e);
                MSOutlook.Application OL = new MSOutlook.Application();
                var selection = OL.ActiveExplorer().Selection;
                if (string.IsNullOrEmpty(selectedFileName) ||
                    selection.Count < 1)
                {
                    throw new DesktopException(
                        "Unexpected state: Drag event is from Outlook " +
                        "but has no selection.");
                }
                var obj = selection[1];
                Marshal.ReleaseComObject(selection);
                if (obj is MSOutlook.MailItem mailitem)
                {
                    bool draggedItemIsAttachment = false;
                    var attachments = mailitem.Attachments;
                    foreach (MSOutlook.Attachment attachment in attachments)
                    {
                        if (selectedFileName == attachment.FileName)
                        {
                            filename = $"{GlobalConstants.APPLICATION_TEMP_FOLDER}{selectedFileName}";
                            draggedItemIsAttachment = true;
                            attachment.SaveAsFile(filename);
                            Marshal.ReleaseComObject(attachment);
                            break;
                        }
                    }
                    Marshal.ReleaseComObject(attachments);
                    if (!draggedItemIsAttachment)
                    {
                        filename = $"{GlobalConstants.APPLICATION_TEMP_FOLDER}" +
                            $"{selectedFileName.ToValidFileName()}";
                        mailitem.SaveAs(filename);
                        Marshal.ReleaseComObject(mailitem);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Did you try to upload a calendar item, task " +
                        "or note? \nBAD USER! \nGO SIT ON THE NAUGHTY STEP!",
                        "What have you done now?",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                try
                {
                    Marshal.ReleaseComObject(obj);
                }
                catch (InvalidComObjectException ex)
                {
                    _log.Warn("Error releasing COM object. " +
                        "(Typically this occurs when an " +
                        "email object has been dropped. " +
                        $"Error: {ex.Message}");
                }
                Marshal.ReleaseComObject(OL);
            }
            return filename;
        }
        private static string GetFileGroupDescriptorFileName(DragEventArgs e)
        {
            StringBuilder sbFileName = new StringBuilder();
            if (e.Data.GetDataPresent("FileGroupDescriptor"))
            {
                Stream theStream = (Stream)e.Data.GetData(
                    "FileGroupDescriptor");
                byte[] fileGroupDescriptor = new byte[512];
                theStream.Read(fileGroupDescriptor, 0, 512);
                for (int i = 76; fileGroupDescriptor[i] != 0; i++)
                {
                    sbFileName.Append(Convert.ToChar(fileGroupDescriptor[i]));
                }
                theStream.Close();
            }
            return sbFileName.ToString();
        }

    }
}
