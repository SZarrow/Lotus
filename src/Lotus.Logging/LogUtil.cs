using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lotus.Logging
{
    public static class LogUtil
    {
        public static String FormatException(Exception ex)
        {
            if (ex == null)
            {
                return String.Empty;
            }

            StringBuilder sb = new StringBuilder();
            String tab = "    ";

            if (ex is AggregateException)
            {
                var ae = (ex as AggregateException).Flatten();
                if (ae.InnerExceptions.Count > 0)
                {
                    foreach (var ie in ae.InnerExceptions)
                    {
                        sb.AppendLine(FormatException(ie));
                    }
                }

                return sb.ToString();
            }
            else
            {
                sb.AppendLine();
                sb.Append(tab);
                sb.AppendFormat("|-- Exception：{0}", ex.Message);
                sb.AppendLine();
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    sb.Append(tab);
                    sb.Append(tab);
                    sb.AppendFormat("|-- InnerException：{0}", innerException.Message);
                    sb.AppendLine();

                    if (innerException.StackTrace != null)
                    {
                        sb.Append(tab);
                        sb.Append(tab);
                        sb.Append(tab);
                        sb.Append("|-- StackTrace：");
                        sb.AppendLine();
                        using (StringReader sr = new StringReader(innerException.StackTrace))
                        {
                            while (sr.Peek() != -1)
                            {
                                sb.Append(tab);
                                sb.Append(tab);
                                sb.Append(tab);
                                sb.Append(tab);
                                sb.AppendFormat("|-- {0}", sr.ReadLine());
                                sb.AppendLine();
                            }
                        }
                    }

                    innerException = innerException.InnerException;
                }
                if (ex.StackTrace != null)
                {
                    sb.Append(tab);
                    sb.Append(tab);
                    sb.Append("|-- StackTrace：");
                    sb.AppendLine();
                    using (StringReader sr = new StringReader(ex.StackTrace))
                    {
                        while (sr.Peek() != -1)
                        {
                            sb.Append(tab);
                            sb.Append(tab);
                            sb.Append(tab);
                            sb.AppendFormat("|-- {0}", sr.ReadLine());
                            sb.AppendLine();
                        }
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
    }
}
