using System;
using System.Collections.Generic;
using System.Text;
using Lotus.MQCore;
using Lotus.MQCore.Common;
using Microsoft.Extensions.Configuration;
using ons;

namespace Lotus.MQProvider.RocketMQ
{
    public class RocketMQProducerFactory : ProducerFactory
    {
        public override IProducer CreateProducer(String producerId, MQMessageType messageType, params Object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(producerId))
            {
                throw new ArgumentNullException(nameof(producerId));
            }

            var config = new ProducerConfig(producerId);
            ONSFactoryProperty onsconfig = new ONSFactoryProperty();
            onsconfig.setFactoryProperty(ONSFactoryProperty.ProducerId, producerId);
            onsconfig.setFactoryProperty(ONSFactoryProperty.AccessKey, config.AccessKeyId);
            onsconfig.setFactoryProperty(ONSFactoryProperty.SecretKey, config.AccessKeySecret);

            switch (messageType)
            {
                case MQMessageType.GeneralMessage:

                    var producer = ONSFactory.getInstance().createProducer(onsconfig);

                    if (producer != null)
                    {
                        try
                        {
                            producer.start();
                        }
                        catch { }
                    }

                    return new RocketMQProducer(producer);

                case MQMessageType.OrderMessage:

                    var orderProducer = ONSFactory.getInstance().createOrderProducer(onsconfig);

                    if (orderProducer != null)
                    {
                        try
                        {
                            orderProducer.start();
                        }
                        catch { }
                    }

                    return new RocketMQProducer(orderProducer);

                case MQMessageType.TransactionMessage:

                    if (parameters == null || parameters.Length == 0)
                    {
                        throw new ArgumentNullException(nameof(parameters));
                    }

                    var checker = parameters[0] as LocalTransactionChecker;
                    if (checker == null)
                    {
                        throw new FormatException("parameters[0] is not LocalTransactionChecker");
                    }

                    var transProducer = ONSFactory.getInstance().createTransactionProducer(onsconfig, checker);

                    if (transProducer != null)
                    {
                        try
                        {
                            transProducer.start();
                        }
                        catch { }
                    }

                    return new RocketMQProducer(transProducer);
            }

            return null;
        }

        public static Object CreateTransactionMessageParameter(Func<Message, TransactionStatus> checker)
        {
            if (checker == null)
            {
                throw new ArgumentNullException(nameof(checker));
            }

            return new RocketMQTransactionChecker(checker);
        }
    }

    internal class RocketMQTransactionChecker : LocalTransactionChecker
    {
        private Func<Message, TransactionStatus> _checker;

        public RocketMQTransactionChecker(Func<Message, TransactionStatus> checker)
        {
            if (checker == null)
            {
                throw new ArgumentNullException("checker");
            }

            _checker = checker;
        }

        public override TransactionStatus check(Message msg)
        {
            return _checker(msg);
        }
    }
}
