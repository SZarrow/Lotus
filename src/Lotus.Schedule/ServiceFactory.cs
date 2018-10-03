using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Lotus.Schedule
{
    /// <summary>
    /// 服务工厂类，用来创建Windows服务。
    /// </summary>
    public static class ServiceFactory
    {
        /// <summary>
        /// 运行服务，如果服务不存在则创建一个。
        /// </summary>
        /// <param name="service">服务描述</param>
        /// <param name="jobsConfigFilePath">任务配置文件，具体参见bin目录下的QuartzJobs.config文件。</param>
        public static void RunService(ServiceDescription service, String jobsConfigFilePath)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (String.IsNullOrWhiteSpace(jobsConfigFilePath))
            {
                throw new ArgumentNullException(nameof(jobsConfigFilePath));
            }

            if (!File.Exists(jobsConfigFilePath))
            {
                throw new FileNotFoundException(jobsConfigFilePath);
            }

            HostFactory.Run(x =>
            {
                x.Service(() =>
                {
                    return new QuartzService(jobsConfigFilePath);
                });

                x.SetDescription(service.Description);
                x.SetDisplayName(service.DisplayName);
                x.SetServiceName(service.ServiceName);
                x.EnablePauseAndContinue();
            });
        }
    }
}
