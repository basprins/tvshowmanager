using System;
using System.IO;
using log4net;

namespace PerfectCode.Logging
{
    public class Logger : ILogger
    {
        private readonly ILog _logger;

        public static void Initialize(string path)
        {
            GlobalContext.Properties["LogName"] = "TV Show Manager";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(path));
        }

        public Logger(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.DebugFormat(message, args);
        }

        public void Info(string message, params object[] args)
        {
            _logger.InfoFormat(message, args);
        }

        public void Error(string message, params object[] args)
        {
            _logger.ErrorFormat(message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            _logger.FatalFormat(message, args);
        }
    }
}
