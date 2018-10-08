using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using Topshelf;

namespace Lotus.Schedule
{
    internal class QuartzService : ServiceControl, ServiceSuspend
    {
        private readonly IScheduler scheduler;
        private FileSystemWatcher watcher = new FileSystemWatcher();
        private Dictionary<JobKey, JobInfo> cachedJobs = new Dictionary<JobKey, JobInfo>(30);
        private String _jobsConfigFilePath;

        public QuartzService(String jobsConfigFilePath)
        {
            if (String.IsNullOrWhiteSpace(jobsConfigFilePath))
            {
                throw new ArgumentNullException(nameof(jobsConfigFilePath));
            }

            if (!File.Exists(jobsConfigFilePath))
            {
                throw new FileNotFoundException(jobsConfigFilePath);
            }

            _jobsConfigFilePath = jobsConfigFilePath;
            scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
        }

        public Boolean Continue(HostControl hostControl)
        {
            scheduler.ResumeAll();
            return true;
        }

        public Boolean Pause(HostControl hostControl)
        {
            scheduler.PauseAll();
            return true;
        }

        public Boolean Start(HostControl hostControl)
        {
            if (scheduler != null && !scheduler.IsStarted)
            {
                var doc = LoadConfig(_jobsConfigFilePath);
                InitJobs(doc);
                WatchConfig(_jobsConfigFilePath);
                scheduler.Start();
            }

            return true;
        }

        public Boolean Stop(HostControl hostControl)
        {
            if (scheduler != null && !scheduler.IsShutdown)
            {
                scheduler.Shutdown();
            }

            return true;
        }

        /// <summary>
        /// 初始化任务
        /// </summary>
        /// <param name="jobEl"></param>
        private void InitJobs(XDocument doc)
        {
            if (doc == null) { return; }

            cachedJobs.Clear();

            foreach (var jobEl in doc.Root.Elements())
            {
                var jobInfo = ParseJob(jobEl);
                if (jobInfo == null) { continue; }

                var jobKey = jobInfo.JobDetail.Key;
                scheduler.ScheduleJob(jobInfo.JobDetail, jobInfo.Trigger);
                cachedJobs.Add(jobKey, jobInfo);

                var switchAttr = jobEl.Element("switch");
                String switchString = switchAttr != null ? switchAttr.Value : String.Empty;
                if (switchString.ToUpperInvariant() == "OFF")
                {
                    scheduler.PauseJob(jobKey);
                }
                else if (switchString.ToUpperInvariant() == "ON")
                {
                    scheduler.ResumeJob(jobKey);
                }
            }
        }

        private static JobInfo ParseJob(XElement jobEl)
        {
            var jobName = jobEl.Attribute("name").Value;
            var jobTypeString = jobEl.Attribute("type").Value;
            var cron = jobEl.Element("cron").Value;

            String[] typeDef = jobTypeString.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (typeDef.Length < 2)
            {
                Console.WriteLine($"任务 {jobName} 的type属性配置错误！");
                return null;
            }

            Type jobType = null;
            try
            {
                var asm = Assembly.Load(typeDef[1].Trim());
                jobType = asm.GetType(typeDef[0].Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载任务 {jobName} 失败：" + ex.Message);
                return null;
            }

            if (jobType == null)
            {
                Console.WriteLine($"无法加载任务类型 {jobName}，请确认代码是否有误！");
                return null;
            }

            IJobDetail jobDetail = JobBuilder.Create(jobType)
                   .WithIdentity(jobName)
                   .Build();

            ICronTrigger jobTrigger = (ICronTrigger)TriggerBuilder.Create()
               .WithIdentity(jobName + "Trigger")
               .StartNow()
               .WithCronSchedule(cron, b => b.WithMisfireHandlingInstructionDoNothing())
               .Build();

            return new JobInfo()
            {
                JobDetail = jobDetail,
                Trigger = jobTrigger
            };
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="jobsEl"></param>
        private void UpdateJobs(XDocument doc)
        {
            if (doc == null) { return; }

            var newJobs = new List<JobKey>();

            foreach (var jobEl in doc.Root.Elements())
            {
                var jobInfo = ParseJob(jobEl);
                if (jobInfo == null) { continue; }

                newJobs.Add(jobInfo.JobDetail.Key);

                if (!cachedJobs.ContainsKey(jobInfo.JobDetail.Key))
                {
                    scheduler.ScheduleJob(jobInfo.JobDetail, jobInfo.Trigger);
                    cachedJobs.Add(jobInfo.JobDetail.Key, jobInfo);
                }
                else
                {
                    var jobKey = jobInfo.JobDetail.Key;
                    var cachedJob = cachedJobs[jobKey];
                    Boolean needUpdate = false;

                    if (jobInfo.JobDetail.JobType != cachedJob.JobDetail.JobType)
                    {
                        needUpdate = true;
                    }

                    if ((jobInfo.Trigger as CronTriggerImpl).CronExpressionString !=
                        (cachedJob.Trigger as CronTriggerImpl).CronExpressionString)
                    {
                        needUpdate = true;
                    }

                    if (needUpdate)
                    {
                        scheduler.PauseJob(jobKey);
                        scheduler.DeleteJob(jobKey);
                        scheduler.ScheduleJob(jobInfo.JobDetail, jobInfo.Trigger);
                        cachedJobs[jobKey] = jobInfo;
                    }

                    var switchAttr = jobEl.Element("switch");
                    String switchString = switchAttr != null ? switchAttr.Value : String.Empty;
                    if (switchString.ToUpperInvariant() == "OFF")
                    {
                        scheduler.PauseJob(jobKey);
                    }
                    else if (switchString.ToUpperInvariant() == "ON")
                    {
                        scheduler.ResumeJob(jobKey);
                    }
                }
            }

            //删除配置文件中没有的任务
            if (cachedJobs.Values != null)
            {
                var deletingJobKeys = (from t0 in cachedJobs
                                       where !newJobs.Contains(t0.Key)
                                       select t0.Key).ToList();

                if (deletingJobKeys != null)
                {
                    foreach (var jobKey in deletingJobKeys)
                    {
                        scheduler.PauseJob(jobKey);
                        scheduler.DeleteJob(jobKey);
                        cachedJobs.Remove(jobKey);
                    }
                }
            }
        }

        /// <summary>
        /// 监视配置文件的改动
        /// </summary>
        /// <param name="configFilePath"></param>
        private void WatchConfig(String configFilePath)
        {
            watcher.Path = Path.GetDirectoryName(configFilePath);
            watcher.Filter = Path.GetFileName(configFilePath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (Object sender, FileSystemEventArgs e) =>
            {
                Thread.Sleep(2000);
                watcher.EnableRaisingEvents = false;
                UpdateJobs(LoadConfig(e.FullPath));
                watcher.EnableRaisingEvents = true;
            };

            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        private XDocument LoadConfig(String jobsConfigFile)
        {
            if (!File.Exists(jobsConfigFile))
            {
                Console.WriteLine($"任务配置文件 {jobsConfigFile} 不存在！");
                return null;
            }

            try
            {
                return XDocument.Load(jobsConfigFile, LoadOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取配置文件失败：" + ex.Message);
                return null;
            }
        }

        class JobInfo
        {
            public IJobDetail JobDetail { get; set; }
            public ITrigger Trigger { get; set; }
        }
    }
}
