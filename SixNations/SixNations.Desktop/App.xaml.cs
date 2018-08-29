using log4net;
using System;
using System.Reflection;
using System.Windows;
using SixNations.Desktop.ViewModels;
using SixNations.Desktop.Views;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Threading;

namespace SixNations.Desktop
{
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static MainWindow app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Info("Application Startup");

            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            DispatcherHelper.Initialize();

            app = new MainWindow();
            var context = ServiceLocator.Current.GetInstance<MainViewModel>();
            app.Show();
        }

        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            // Here change path to the log.txt file
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                + "\\SixNations.Desktop\\log.txt";

            // Show a message before closing application
            
            MessageBox.Show(
                "Oops, something went wrong and the application must close. Please find a " +
                $"report on the issue at: {path}{Environment.NewLine}" +
                "If the problem persist, please contact David Savage.",
                "Unhandled Error",
                MessageBoxButton.OK);

            Exception e = (Exception)args.ExceptionObject;
            Log.Fatal("Application has crashed", e);
        }
    }
}
