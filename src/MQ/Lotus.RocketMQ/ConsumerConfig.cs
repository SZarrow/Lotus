using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.MQProvider.RocketMQ
{
    [Serializable]
    public class ConsumerConfig : AliyunONSConfig
    {
        private readonly String _consumerId;

        public ConsumerConfig(String consumerId)
        {
            if (String.IsNullOrWhiteSpace(consumerId))
            {
                throw new ArgumentNullException(nameof(consumerId));
            }

            _consumerId = consumerId;
        }

        public override String MQRoleId
        {
            get
            {
                return _consumerId;
            }
        }

        public override String AccessKeyId
        {
            get
            {
                return Configuration["ONSSub:AccessKeyId"];
            }
        }

        public override String AccessKeySecret
        {
            get
            {
                return Configuration["ONSSub:AccessKeySecret"];
            }
        }
    }
}
