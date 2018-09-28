using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lotus.Scheduler.Web.Common
{
    public class JobConfig
    {
        static JobConfig()
        {
            JobsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Jobs");
        }

        public static String JobsDir { get; }
    }
}
