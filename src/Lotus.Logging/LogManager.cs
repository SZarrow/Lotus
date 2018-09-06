using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace Lotus.Logging
{
    public static class LogManager
    {
        private static ReaderWriterLockSlim s_lock;
        private static ILogger s_logger;

        static LogManager()
        {
            s_lock = new ReaderWriterLockSlim();
        }

        public static ILogger GetLogger(Action<LoggerBuilder> configAction)
        {
            String appName = "Lotus.Logging";

            var cachedLogger = AppDomain.CurrentDomain.GetData(appName) as ILogger;
            if (cachedLogger != null) { return cachedLogger; }

            try
            {
                s_lock.EnterWriteLock();

                cachedLogger = AppDomain.CurrentDomain.GetData(appName) as ILogger;
                if (cachedLogger != null) { return cachedLogger; }

                var builder = new LoggerBuilder();
                if (configAction != null)
                {
                    configAction(builder);
                }

                var logger = new TimerLogger(appName, builder.Build());
                s_logger = logger;

                AppDomain.CurrentDomain.SetData(appName, s_logger);

                return s_logger;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (s_lock.IsWriteLockHeld)
                {
                    s_lock.ExitWriteLock();
                }
            }
        }

        public static ILogger GetLogger(LogLevel logLevel)
        {
            return GetLogger(builder =>
           {
               builder.UseFileLogger(config =>
               {
                   config.LogLevel = logLevel;
               });
           });
        }

        public static ILogger GetLogger()
        {
            return GetLogger(LogLevel.Error | LogLevel.Warn | LogLevel.Info | LogLevel.Debug | LogLevel.Trace);
        }

    }
}
