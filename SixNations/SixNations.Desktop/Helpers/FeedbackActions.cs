using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace SixNations.Desktop.Helpers
{
    static class FeedbackActions
    {
        public static void ReactToException(Exception ex)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var msg = $"{ex.GetType().Name} : {ex.Message} {Environment.NewLine}" +
                        $"Source : {ex.Source}";
                MessageBox.Show(
                        msg,
                        "Action Aborted",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            });
        }
    }
}
