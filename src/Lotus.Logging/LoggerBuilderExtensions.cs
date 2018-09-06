using System;
using System.Collections.Generic;
using System.IO;

namespace Lotus.Logging
{
    public static class LoggerBuilderExtensions
    {
        public static void UseFileLogger(this LoggerBuilder builder, Action<FileLogConfig> configAction = null)
        {
            FileLogConfig fileConfig = new FileLogConfig()
            {
                FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log", $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt"),
                LogLevel = (LogLevel.Error | LogLevel.Warn | LogLevel.Info | LogLevel.Debug | LogLevel.Trace)
            };

            if (configAction != null)
            {
                configAction(fileConfig);
            }

            builder.AddLogConfig(fileConfig);

            String saveDir = Path.GetDirectoryName(fileConfig.FilePath);
            if (!Directory.Exists(saveDir))
            {
                try
                {
                    Directory.CreateDirectory(saveDir);
                }
                catch (Exception ex) { throw ex; }
            }
        }
    }
}
