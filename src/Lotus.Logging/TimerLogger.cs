using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Lotus.Logging.Appenders;

namespace Lotus.Logging
{
    internal class TimerLogger : ILogger
    {
        private Dictionary<Int32, ConcurrentQueue<LogEntity>> _messages;
        private IEnumerable<IAppender> _appenders;
        private String _appName;

        private IEnumerable<Int32> _logTypes = null;
        private volatile Int32 _dueTime = 1000;
        private Int32 _batchCommitMaxSize = 10;
        private Int32 _idleTimes = 0;

        private Timer _timer;

        public TimerLogger(String appName, IEnumerable<LogConfig> configs)
        {
            if (String.IsNullOrWhiteSpace(appName))
            {
                throw new ArgumentNullException(nameof(appName));
            }

            if (configs == null || configs.Count() == 0)
            {
                throw new ArgumentNullException(nameof(configs));
            }

            _appName = appName;

            var values = Enum.GetValues(typeof(LogType));
            List<Int32> logTypes = new List<Int32>(values.Length);
            foreach (var value in values)
            {
                logTypes.Add((Int32)value);
            }
            _logTypes = logTypes;

            _messages = new Dictionary<Int32, ConcurrentQueue<LogEntity>>(_logTypes.Count());
            _appenders = CreateAppenders(configs);

            InitMessageQueues();
            InitTimer();
        }

        /*---------------------------------------------------------------------------------------*/

        public void Log(String content)
        {
            Log(LogType.Info, content);
        }
        public void Warn(String content)
        {
            Log(LogType.Warn, content);
        }
        public void Error(String content)
        {
            Error(null, content);
        }
        public void Error(Exception ex, String subject = null)
        {
            Log(LogType.Error, subject, ex);
        }

        public void Log(LogType logType, String content, params Exception[] exceptions)
        {
            AppendToQueue(logType, new LogEntity()
            {
                Id = Guid.NewGuid(),
                Exceptions = exceptions,
                LogContent = content,
                LogType = logType,
                CreateTime = DateTime.Now
            });
        }

        /*---------------------------------------------------------------------------------------*/

        private void AppendToQueue(LogType logType, LogEntity log)
        {
            Int32 key = (Int32)logType;

            if (log != null)
            {
                _messages[key].Enqueue(log);
            }
        }

        private void InitTimer()
        {
            _timer = new Timer(TimerTick, null, Timeout.Infinite, Timeout.Infinite);
            _timer.Change(_dueTime, Timeout.Infinite);
        }

        private IEnumerable<IAppender> CreateAppenders(IEnumerable<LogConfig> logConfigs)
        {
            foreach (var config in logConfigs)
            {
                if (config is FileLogConfig)
                {
                    yield return new FileAppender(config as FileLogConfig);
                }
            }
        }

        private void InitMessageQueues()
        {
            foreach (var logType in _logTypes)
            {
                _messages[logType] = new ConcurrentQueue<LogEntity>();
            }
        }

        private void TimerTick(Object state)
        {
            foreach (Int32 logType in _logTypes)
            {
                if (_messages.ContainsKey(logType) && _messages[logType].Count > 0)
                {
                    Interlocked.Exchange(ref _idleTimes, 0);

                    List<LogEntity> entities = new List<LogEntity>();

                    while (_messages[logType].Count > 0)
                    {
                        LogEntity entity;

                        _messages[logType].TryDequeue(out entity);

                        if (entity != null)
                        {
                            entities.Add(entity);
                        }

                        if (entities.Count == _batchCommitMaxSize)
                        {
                            WriteToAppenders(logType, entities);
                            entities.Clear();
                        }
                    }

                    if (entities.Count > 0)
                    {
                        WriteToAppenders(logType, entities);
                        entities.Clear();
                    }
                }
            }

            Interlocked.Add(ref _idleTimes, 1);

            if (_idleTimes >= 1 && _idleTimes <= 6)
            {
                _dueTime = _idleTimes * 1000;
            }

            _timer.Change(_dueTime, Timeout.Infinite);
        }

        private void WriteToAppenders(Int32 logType, IEnumerable<LogEntity> entities)
        {
            foreach (var appender in _appenders)
            {
                var writeResult = appender.Write((LogType)logType, entities);
                if (!writeResult.Success && writeResult.Value != null)
                {
                    var failedLogs = entities.Except(writeResult.Value);
                    foreach (var failedLog in failedLogs)
                    {
                        AppendToQueue((LogType)logType, failedLog);
                    }
                }
            }
        }
    }
}
