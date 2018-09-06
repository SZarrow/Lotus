using System;
using System.Collections.Generic;
using Lotus.Core;

namespace Lotus.Logging
{
    /// <summary>
    /// 同步Appender接口
    /// </summary>
    public interface IAppender
    {
        /// <summary>
        /// 获取当前Appender的日志级别
        /// </summary>
        LogLevel LogLevel { get; }
        /// <summary>
        /// 以同步的方式写入指定类型的日志
        /// </summary>
        /// <param name="logType">写入的日志的类型</param>
        /// <param name="contents">写入的日志的内容</param>
        XResult<IEnumerable<LogEntity>> Write(LogType logType, IEnumerable<LogEntity> contents);
    }
}
