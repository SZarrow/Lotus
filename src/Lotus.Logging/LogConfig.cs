using System;
using System.Collections.Generic;

namespace Lotus.Logging
{
    public abstract class LogConfig
    {
        protected LogConfig() { }

        public LogLevel LogLevel { get; set; }
    }

    public class FileLogConfig : LogConfig, IEquatable<FileLogConfig>
    {
        public String FilePath { get; set; }

        public Boolean Equals(FileLogConfig other)
        {
            if (other == null) { return false; }
            return this.GetHashCode() == other.GetHashCode();
        }

        public override Int32 GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override String ToString()
        {
            return $"{FilePath}:{((Int32)this.LogLevel).ToString()}";
        }
    }
}
