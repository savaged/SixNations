using savaged.mvvm.Data;
using GalaSoft.MvvmLight.Threading;
using log4net;
using System;
using System.Media;
using System.Reflection;
using System.Windows;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class ExceptionHandlingService
    {
        private static readonly ILog _log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        public void ReactToException(object origin, DesktopException ex)
        {
            var title = "Action Aborted";
            var msg = string.Empty;

            if (ex is ApiAuthException aex)
            {
                _log.Warn($"{title}! Error in {origin} : {aex}");
                return;
            }
            else if (ex is ApiValidationException vex)
            {
                title += ": Input Validation Failure";
                msg = $"{ex.Message}";
            }
            else
            {
                title += ": Error";
                msg += "Something went wrong! " +
                    $"You may need to refresh or restart. {Environment.NewLine}" +
                    $"{ex.GetType().Name} : {ex.Message} {Environment.NewLine}" +
                    $"Origin: {origin.ToString()} Source: {ex.Source}";
            }

            const string apiNewLine = "\\r\\n";
            if (!string.IsNullOrEmpty(msg) &&  msg.Contains(apiNewLine))
            {
                msg = msg.Replace(apiNewLine, Environment.NewLine);
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (ex.InnerException != null)
                {
                    msg += $"{Environment.NewLine}Inner: {ex.Message}" +
                        $"{Environment.NewLine}{ex.StackTrace}";
                }
                SystemSounds.Hand.Play();
                MessageBox.Show(
                        msg,
                        title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
            });
        }

        public void DontReactToException(object origin, DesktopException ex)
        {
            _log.Warn($"Ignored error in {origin} : {ex.Message}");
        }

    }
}
