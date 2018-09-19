using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace SixNations.Desktop.Helpers
{
    public static class FeedbackActions
    {
        public static void ReactToException(Exception ex)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var msg = $"{ex.GetType().Name} : {ex.Message} {Environment.NewLine}" +
                        $"Source : {ex.Source} {Environment.NewLine}" +
#if DEBUG
                        $"Stack Trace : {ex.StackTrace} {Environment.NewLine}" +
#endif
                        $"See log for more detail (found here: {Environment.NewLine}" +
                        $"{LogFileLocator.GetLogFileLocation()})";
                MessageBox.Show(
                        msg,
                        "Action Aborted",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            });
        }
    }
}
