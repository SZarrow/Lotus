using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lotus.Scheduler.Web.Common;
using Lotus.Scheduler.Web.Models;
using Quartz;
using Quartz.Impl;

namespace Lotus.Scheduler.Web.Services
{
    public class ScheduleService
    {
        private readonly static StdSchedulerFactory s_scheduleFactory;

        static ScheduleService()
        {
            s_scheduleFactory = new StdSchedulerFactory(new NameValueCollection() {
                { "quartz.serializer.type", "binary"}
            });
        }

        public IEnumerable<JobListItem> GetJobListItems(Int32 pageIndex, Int32 pageSize, String keyword = null)
        {
            List<JobListItem> items = new List<JobListItem>(20);

            var runningJobs = GetRunningJobs();

            var jobDirs = Directory.EnumerateDirectories(JobConfig.JobsDir);
            foreach (var jobDir in jobDirs)
            {
                var dirInfo = new DirectoryInfo(jobDir);
                var kv = GetJobNameAndVersion(dirInfo.Name);

                var item = new JobListItem()
                {
                    JobName = kv.Key,
                    Version = kv.Value,
                    UpdateTime = dirInfo.LastWriteTime
                };

                var jobIsRunning = runningJobs.Count(x => x.JobName == item.JobName && x.Group == item.Group) > 0;
                if (jobIsRunning)
                {
                    item.JobStatus = JobStatus.Running;
                }

                items.Add(item);
            }

            return items;
        }

        public IEnumerable<JobListItem> GetRunningJobs()
        {
            var runningJobs = new List<JobListItem>(20);

            var schedulers = s_scheduleFactory.GetAllSchedulers().GetAwaiter().GetResult();
            if (schedulers != null)
            {
                foreach (var sche in schedulers)
                {
                    var jobs = sche.GetCurrentlyExecutingJobs().GetAwaiter().GetResult();
                    if (jobs != null)
                    {
                        foreach (var job in jobs)
                        {
                            var triggerState = sche.GetTriggerState(job.Trigger.Key).GetAwaiter().GetResult();
                            var kv = GetJobNameAndVersion(job.JobDetail.Key.Name);
                            runningJobs.Add(new JobListItem()
                            {
                                JobStatus = GetJobStatus(triggerState),
                                JobName = kv.Key,
                                Group = job.JobDetail.Key.Group,
                                Version = kv.Value,
                                 JobDescription = job.JobDetail.Description
                            });
                        }
                    }
                }
            }

            return runningJobs;
        }

        private KeyValuePair<String, String> GetJobNameAndVersion(String jobId)
        {
            var match = Regex.Match(jobId, @"([\w\.]+?)((\.\d+)*)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return new KeyValuePair<String, String>(match.Groups[1].Value, match.Groups[2].Value.Substring(1));
            }

            return new KeyValuePair<String, String>(jobId, String.Empty);
        }

        private JobStatus GetJobStatus(TriggerState state)
        {
            switch (state)
            {
                case TriggerState.Blocked:
                case TriggerState.Normal:
                    return JobStatus.Running;
                case TriggerState.Complete:
                case TriggerState.Error:
                    return JobStatus.Stoped;
                case TriggerState.Paused:
                    return JobStatus.Pause;
            }

            return JobStatus.None;
        }
    }
}
