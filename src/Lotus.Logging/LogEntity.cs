using System;
using System.Text;
using Lotus.Core;

namespace Lotus.Logging
{
    [Serializable]
    public class LogEntity : IEquatable<LogEntity>
    {
        private StringBuilder _sb;

        public Guid Id { get; set; }
        public LogType LogType { get; set; }
        public String LogContent { get; set; }
        public Exception[] Exceptions { get; set; }
        public DateTime CreateTime { get; set; }

        public LogEntity()
        {
            _sb = new StringBuilder();
        }

        public Boolean Equals(LogEntity other)
        {
            if (other == null) { return false; }

            if (!(other is LogEntity) && !other.GetType().IsSubclassOf(typeof(LogEntity)))
            {
                return false;
            }

            return (other as LogEntity).Id == this.Id;
        }

        public override Int32 GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override String ToString()
        {
            _sb.Clear();

            _sb.Append($"[{this.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff")}] ");
            _sb.Append(this.LogContent);

            if (this.Exceptions != null && this.Exceptions.Length > 0)
            {
                foreach (var ex in this.Exceptions)
                {
                    _sb.AppendLine(LogUtil.FormatException(ex));
                }
            }

            return _sb.ToString();
        }
    }
}
