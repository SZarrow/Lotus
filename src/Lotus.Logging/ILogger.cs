using System;
using System.Collections.Generic;

namespace Lotus.Logging
{
    public interface ILogger
    {
        //void Log(String content);
        //void Warn(String content);
        void Error(String content);
        void Error(Exception ex, String subject = null);
    }
}
