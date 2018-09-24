using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lotus.Scheduler.Web.Models
{
    public enum JobStatus
    {
        None,
        Running,
        Stoped,
        Recovering,
        Pause
    }
}
