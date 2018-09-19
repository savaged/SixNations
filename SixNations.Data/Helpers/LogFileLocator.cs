using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System.Linq;
using System.Reflection;

namespace SixNations.Data.Helpers
{
    public static class LogFileLocator
    {
        public static string GetLogFileLocation()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            var rootAppender = ((Hierarchy)LogManager.GetRepository(entryAssembly))
                                         .Root.Appenders.OfType<FileAppender>()
                                         .FirstOrDefault();
            string filename = rootAppender != null ? rootAppender.File : string.Empty;
            return filename;
        }
    }
}
