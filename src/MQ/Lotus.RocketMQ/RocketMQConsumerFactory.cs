using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lotus.MQCore;
using Lotus.MQCore.Common;
using ons;

namespace Lotus.MQProvider.RocketMQ
{
    public class RocketMQConsumerFactory : ConsumerFactory
    {
        public override IConsumer CreateConsumer(String consumerId, SubscribeType subscribeType, params Object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(consumerId))
            {
                throw new ArgumentNullException(nameof(consumerId));
            }

            var config = new ConsumerConfig(consumerId);
            ONSFactoryProperty onsconfig = new ONSFactoryProperty();
            onsconfig.setFactoryProperty(ONSFactoryProperty.ConsumerId, consumerId);
            onsconfig.setFactoryProperty(ONSFactoryProperty.AccessKey, config.AccessKeyId);
            onsconfig.setFactoryProperty(ONSFactoryProperty.SecretKey, config.AccessKeySecret);

            String logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
            if (!Directory.Exists(logDir))
            {
                try
                {
                    Directory.CreateDirectory(logDir);
                    onsconfig.setFactoryProperty(ONSFactoryProperty.LogPath, logDir);
                }
                catch { }
            }

            switch (subscribeType)
            {
                case SubscribeType.CLUSTERING:
                    onsconfig.setFactoryProperty(ONSFactoryProperty.MessageModel, ONSFactoryProperty.CLUSTERING);
                    break;

                case SubscribeType.BROADCASTING:
                    onsconfig.setFactoryProperty(ONSFactoryProperty.MessageModel, ONSFactoryProperty.CLUSTERING);
                    break;
            }

            var pushConsumer = ONSFactory.getInstance().createPushConsumer(onsconfig);
            return new RocketMQConsumer(pushConsumer);
        }
    }
}
