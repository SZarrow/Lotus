using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Logging
{
    [Flags]
    public enum LogLevel
    {
        None = 0x00000,
        Trace = 0x00001,
        Debug = 0x00010,
        Info = 0x00100,
        Warn = 0x01000,
        Error = 0x10000
    }
}
