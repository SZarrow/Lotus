using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Logging
{
    public abstract class Appender
    {
        protected Appender() { }

        /// <summary>
        /// 根据日志级别判断是否允许记录指定类型的日志
        /// </summary>
        /// <param name="allowedLogLevel">表示Appender允许记录的日志级别</param>
        /// <param name="logType">表示Apender将要记录的日志类型</param>
        protected Boolean AllowAppend(LogLevel allowedLogLevel, LogType logType)
        {
            return ((Int32)allowedLogLevel & (Int32)logType) == (Int32)logType;
        }
    }
}
