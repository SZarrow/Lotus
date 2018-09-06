using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.MQProvider.RocketMQ
{
    [Serializable]
    public class ProducerConfig : AliyunONSConfig
    {
        private readonly String _producerId;

        public ProducerConfig(String producerId)
        {
            if (String.IsNullOrWhiteSpace(producerId))
            {
                throw new ArgumentNullException("producerId");
            }

            _producerId = producerId;
        }

        public override String MQRoleId
        {
            get
            {
                return _producerId;
            }
        }

        public override String AccessKeyId
        {
            get
            {
                return Configuration["ONSPub:AccessKeyId"];
            }
        }

        public override String AccessKeySecret
        {
            get
            {
                return Configuration["ONSPub:AccessKeySecret"];
            }
        }
    }
}
