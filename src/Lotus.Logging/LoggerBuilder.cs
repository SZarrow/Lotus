using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Logging
{
    public class LoggerBuilder
    {
        private List<LogConfig> _configs;

        public LoggerBuilder()
        {
            _configs = new List<LogConfig>(2);
        }

        internal void AddLogConfig(LogConfig config)
        {
            if (config == null) { return; }
            _configs.Add(config);
        }

        public IEnumerable<LogConfig> Build()
        {
            return _configs;
        }
    }
}
