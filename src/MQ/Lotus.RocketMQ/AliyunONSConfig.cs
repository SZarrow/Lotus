using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Lotus.MQProvider.RocketMQ
{
    [Serializable]
    public abstract class AliyunONSConfig
    {
        private static readonly IConfiguration s_config;
        private static readonly ReaderWriterLockSlim s_locker = new ReaderWriterLockSlim();

        static AliyunONSConfig()
        {
            var configBuilder = new ConfigurationBuilder();

            String configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AliyunONSConfig.json");
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException(configFile);
            }

            var config = configBuilder.AddJsonFile(configFile).Build();
            if (config["TEST"] == "True")
            {

                configFile = @"E:\Configs\AliyunONSConfig.json";
                if (!File.Exists(configFile))
                {
                    throw new FileNotFoundException(configFile);
                }

                configBuilder.Sources.RemoveAt(0);
                config = configBuilder.AddJsonFile(configFile).Build();
            }

            s_locker.EnterWriteLock();
            s_config = config;
            s_locker.ExitWriteLock();
        }

        protected AliyunONSConfig()
        {
            if (s_config != null)
            {
                this.ONSUrl = s_config["ONSUrl"];
            }
        }

        protected IConfiguration Configuration
        {
            get
            {
                return s_config;
            }
        }

        /// <summary>
        /// 这个属性当被生产者配置类实现之后就表示生产者的Id，当被消费者配置类实现之后就表示消费者的Id。
        /// </summary>
        public abstract String MQRoleId { get; }
        public virtual String AccessKeyId { get; }
        public virtual String AccessKeySecret { get; }
        public String ONSUrl { get; }
    }
}
