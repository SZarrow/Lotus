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
        private readonly static Dictionary<String, JobItem> s_jobDic;

        static ScheduleService()
        {
            s_scheduleFactory = new StdSchedulerFactory(new NameValueCollection() {
                { "quartz.serializer.type", "binary"}
            });

            s_jobDic = new Dictionary<String, JobItem>();
        }

        public IEnumerable<JobItem> GetRunningJobs()
        {
            var runningJobs = new List<JobItem>(20);

            var schedulers = s_scheduleFactory.GetAllSchedulers().GetAwaiter().GetResult();
            if (schedulers != null)
            {
                foreach (var sche in schedulers)
                {
                    var jobs = sche.GetCurrentlyExecutingJobs().GetAwaiter().GetResult();
                    if (jobs == null) { continue; }

                    foreach (var job in jobs)
                    {
                        var triggerState = sche.GetTriggerState(job.Trigger.Key).GetAwaiter().GetResult();
                        var kv = GetJobNameAndVersion(job.JobDetail.Key.Name);
                        runningJobs.Add(new JobItem()
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

        private void LoadJobs()
        {
            var jobDirs = Directory.EnumerateDirectories(JobConfig.JobsDir);
            foreach (var jobDir in jobDirs)
            {
                var dirInfo = new DirectoryInfo(jobDir);
                var nv = GetJobNameAndVersion(dirInfo.Name);

                var jobItem = new JobItem()
                {
                    JobName = nv.Key,
                    Version = nv.Value,
                    UpdateTime = dirInfo.LastWriteTime
                };

                //var jobIsRunning = runningJobs.Count(x => x.JobName == item.JobName && x.Group == item.Group) > 0;
                //if (jobIsRunning)
                //{
                //    item.JobStatus = JobStatus.Running;
                //}

                s_jobDic[jobItem.JobId] = jobItem;
            }
        }

        public void Start()
        {

        }

        public void Shutdown()
        {

        }
    }
}
