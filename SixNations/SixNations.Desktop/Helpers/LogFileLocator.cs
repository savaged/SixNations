using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System.Linq;

namespace SixNations.Desktop.Helpers
{
    public static class LogFileLocator
    {
        public static string GetLogFileLocation()
        {
            var rootAppender = ((Hierarchy)LogManager.GetRepository())
                                         .Root.Appenders.OfType<FileAppender>()
                                         .FirstOrDefault();
            string filename = rootAppender != null ? rootAppender.File : string.Empty;
            return filename;
        }
    }
}
