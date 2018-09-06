using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;
using Lotus.MQCore;
using Lotus.MQCore.Common;
using ons;

namespace Lotus.MQProvider.RocketMQ
{
    public class RocketMQProducer : IProducer
    {
        private readonly Producer _producer;
        private readonly OrderProducer _orderProducer;
        private readonly TransactionProducer _transactionProducer;

        public RocketMQProducer(Producer producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

            _producer = producer;
        }

        public RocketMQProducer(OrderProducer producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

            _orderProducer = producer;
        }

        public RocketMQProducer(TransactionProducer producer)
        {
            if (producer == null)
            {
                throw new ArgumentNullException(nameof(producer));
            }

            _transactionProducer = producer;
        }

        public XResult<Boolean> SendMessage(MQMessage message, params Object[] parameters)
        {
            if (message == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(message)));
            }

            var vr = message.Validate();
            if (!vr.Success)
            {
                return vr;
            }

            if (_producer == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(_producer)));
            }

            try
            {
                _producer.send(new Message(message.PublishTopics, message.Tags, message.MessageContent));
                return new XResult<Boolean>(true);
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }
        }

        public XResult<Boolean> SendOrderMessage(MQMessage message, params Object[] parameters)
        {
            if (message == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(message)));
            }

            var vr = message.Validate();
            if (!vr.Success)
            {
                return vr;
            }

            if (_orderProducer == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(_orderProducer)));
            }

            String shardingKey = (parameters != null && parameters.Length > 0) ? parameters[0].ToString() : Guid.NewGuid().ToString("n");

            try
            {
                _orderProducer.send(new Message(message.PublishTopics, message.Tags, message.MessageContent), shardingKey);
                return new XResult<Boolean>(true);
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }
        }

        public XResult<Boolean> SendTransactionMessage(MQMessage message, params Object[] parameters)
        {
            if (message == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(message)));
            }

            var vr = message.Validate();
            if (!vr.Success)
            {
                return vr;
            }

            if (_transactionProducer == null)
            {
                return new XResult<Boolean>(false, new NullReferenceException(nameof(_transactionProducer)));
            }

            if (parameters == null || parameters.Length == 0)
            {
                return new XResult<Boolean>(false, new ArgumentException(nameof(parameters)));
            }

            var executor = parameters[0] as LocalTransactionExecuter;
            if (executor == null)
            {
                return new XResult<Boolean>(false, new FormatException("parameters[0] is not LocalTransactionExecuter"));
            }

            try
            {
                _transactionProducer.send(new Message(message.PublishTopics, message.Tags, message.MessageContent), executor);
                return new XResult<Boolean>(true);
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }
        }

        public static Object CreateTransactionMessageParameter(Func<Message, TransactionStatus> executor)
        {
            if (executor == null)
            {
                throw new ArgumentNullException(nameof(executor));
            }

            return new RocketMQTransactionExecutor(executor);
        }

        public void Dispose()
        {
            if (_producer != null)
            {
                _producer.shutdown();
                _producer.Dispose();
            }

            if (_orderProducer != null)
            {
                _orderProducer.shutdown();
                _orderProducer.Dispose();
            }

            if (_transactionProducer != null)
            {
                _transactionProducer.shutdown();
                _transactionProducer.Dispose();
            }
        }
    }

    internal class RocketMQTransactionExecutor : LocalTransactionExecuter
    {
        private Func<Message, TransactionStatus> _executor;

        public RocketMQTransactionExecutor(Func<Message, TransactionStatus> executor)
        {
            if (executor == null)
            {
                throw new ArgumentNullException(nameof(executor));
            }

            _executor = executor;
        }

        public override TransactionStatus execute(Message msg)
        {
            return _executor(msg);
        }
    }
}
