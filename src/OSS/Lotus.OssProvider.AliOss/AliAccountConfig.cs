using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lotus.Core;
using Lotus.OssCore.Common;
using Microsoft.Extensions.Configuration;

namespace Lotus.OssProvider.AliOss
{
    /// <summary>
    /// 阿里云api访问账号的配置信息
    /// </summary>
    public class AliAccountConfig
    {
        internal const String WriterRoleId = "ALIOSS_WRITER";
        internal const String ReaderRoleId = "ALIOSS_READER";
        private static readonly Hashtable s_cache = new Hashtable();

        public Uri EndPoint { get; set; }
        public String AccessKeyId { get; set; }
        public String AccessKeySecret { get; set; }

        static AliAccountConfig()
        {
            String configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AliOssAccount.json");
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException(configFile);
            }

            var configBuilder = new ConfigurationBuilder();

            var config = configBuilder.AddJsonFile(configFile).Build();
            if (config["TEST"] == "True")
            {
                configFile = @"D:\Configs\AliOssAccount.json";
                if (!File.Exists(configFile))
                {
                    throw new FileNotFoundException(configFile);
                }

                configBuilder.Sources.RemoveAt(0);
                config = configBuilder.AddJsonFile(configFile).Build();
            }

            if (config != null)
            {
                var writerConfig = new AliAccountConfig()
                {
                    AccessKeyId = config["OSSWriter:AccessKeyId"],
                    AccessKeySecret = config["OSSWriter:AccessKeySecret"],
                    EndPoint = new Uri(config["OSSWriter:EndPoint"])
                };

                ConfigureWriter(writerConfig);

                var readerConfig = new AliAccountConfig()
                {
                    AccessKeyId = config["OSSReader:AccessKeyId"],
                    AccessKeySecret = config["OSSReader:AccessKeySecret"],
                    EndPoint = new Uri(config["OSSReader:EndPoint"])
                };

                ConfigureReader(readerConfig);
            }
        }

        public XResult<Boolean> Validate()
        {
            if (String.IsNullOrWhiteSpace(this.AccessKeyId))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("AccessKeyId"));
            }

            if (String.IsNullOrWhiteSpace(this.AccessKeySecret))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("AccessKeySecret"));
            }

            if (this.EndPoint == null || String.IsNullOrWhiteSpace(this.EndPoint.AbsoluteUri))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("EndPoint"));
            }

            return new XResult<Boolean>(true);
        }

        internal static XResult<AliAccountConfig> GetConfig(String roleId)
        {
            if (s_cache.ContainsKey(roleId))
            {
                return new XResult<AliAccountConfig>(s_cache[roleId] as AliAccountConfig);
            }

            return new XResult<AliAccountConfig>(null, new KeyNotFoundException($"the roleId \"{roleId}\" not found"));
        }

        public static void ConfigureWriter(AliAccountConfig config)
        {
            Configure(WriterRoleId, config);
        }

        public static void ConfigureReader(AliAccountConfig config)
        {
            Configure(ReaderRoleId, config);
        }

        private static void Configure(String roleId, AliAccountConfig config)
        {
            if (!String.IsNullOrWhiteSpace(roleId) && config != null)
            {
                var vr = config.Validate();
                if (vr.Success)
                {
                    s_cache[roleId] = config;
                }
            }
        }
    }
}
