using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lotus.Scheduler.Web.Models
{
    public class JobItem
    {
        public String JobId
        {
            get
            {
                return $"{this.JobName}.{this.Version}";
            }
        }
        public String JobName { get; set; }
        public String Group { get; set; }
        public String JobDescription { get; set; }
        public String Version { get; set; }
        public DateTime UpdateTime { get; set; }
        public JobStatus JobStatus { get; set; }
    }
}