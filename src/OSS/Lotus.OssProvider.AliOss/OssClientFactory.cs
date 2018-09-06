using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Lotus.Core;
using Lotus.OssCore.Common;

namespace Lotus.OssProvider.AliOss
{
    /// <summary>
    /// OssClient工厂类，用来创建OssClient对象。
    /// </summary>
    public class OssClientFactory
    {
        /// <summary>
        /// 获取一个OssClient对象。
        /// </summary>
        public static XResult<OssClient> GetOssClient()
        {
            var result = AliAccountConfig.GetConfig(AliAccountConfig.WriterRoleId);
            if (!result.Success)
            {
                return new XResult<OssClient>(null, result.Exceptions.ToArray());
            }

            return CreateOssClient(result.Value, true);
        }

        private static XResult<OssClient> CreateOssClient(AliAccountConfig account, Boolean useCName)
        {
            var vr = account.Validate();
            if (!vr.Success)
            {
                return new XResult<OssClient>(null, vr.Exceptions.ToArray());
            }

            var clientConfig = new ClientConfiguration()
            {
                IsCname = true
            };

            OssClient client = null;
            try
            {
                client = new OssClient(account.EndPoint, account.AccessKeyId, account.AccessKeySecret, clientConfig);
            }
            catch (Exception ex)
            {
                return new XResult<OssClient>(null, ex);
            }

            return new XResult<OssClient>(client);
        }
    }
}
