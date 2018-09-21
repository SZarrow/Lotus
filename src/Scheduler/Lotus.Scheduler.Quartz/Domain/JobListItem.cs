using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Scheduler.Quartz.Domain
{
    public class JobListItem
    {
        public Boolean Checked { get; set; }
        public String JobName { get; set; }
        public String JobDescription { get; set; }
        public String UpdateTime { get; set; }
        public JobStatus JobStatus { get; set; }
    }

    public enum JobStatus
    {
        Running,
        Stoped
    }
}
