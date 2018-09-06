using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.MQCore.Common
{
    public enum SubscribeType
    {
        /// <summary>
        /// 集群订阅，默认方式
        /// </summary>
        CLUSTERING = 0,
        /// <summary>
        /// 广播订阅
        /// </summary>
        BROADCASTING = 1
    }
}
