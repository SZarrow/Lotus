
using System;
using Lotus.Core;

namespace Lotus.Logging
{
    [Flags]
    public enum LogType
    {
        [EnumDisplayName("Trace")]
        Trace = 0x00001,
        [EnumDisplayName("Debug")]
        Debug = 0x00010,
        [EnumDisplayName("Info")]
        Info = 0x00100,
        [EnumDisplayName("Warn")]
        Warn = 0x01000,
        [EnumDisplayName("Error")]
        Error = 0x10000
    }
}
