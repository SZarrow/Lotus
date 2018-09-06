using System;
using System.Threading;
using Lotus.MQCore;
using Lotus.MQCore.Common;
using ons;
using Xunit;

namespace Lotus.MQProvider.RocketMQ.Tests
{
    public class ProducerFactoryTest
    {
        [Fact]
        public void TestCreateGeneralProducer()
        {
            Thread.Sleep(10);
            ProducerFactory fac = new RocketMQProducerFactory();
            using (var producer = fac.CreateProducer("PID_01", MQMessageType.GeneralMessage))
            {
                Assert.NotNull(producer);
            }
        }

        [Fact]
        public void TestCreateOrderProducer()
        {
            Thread.Sleep(10);
            ProducerFactory fac = new RocketMQProducerFactory();
            using (var producer = fac.CreateProducer("PID_02", MQMessageType.OrderMessage))
            {
                Assert.NotNull(producer);
            }
        }

        [Fact]
        public void TestCreateTransactionProducer()
        {
            Thread.Sleep(10);
            ProducerFactory fac = new RocketMQProducerFactory();
            Func<Message, TransactionStatus> checker = message =>
             {
                 return TransactionStatus.CommitTransaction;
             };

            using (var producer = fac.CreateProducer("PID_03", MQMessageType.TransactionMessage, RocketMQProducerFactory.CreateTransactionMessageParameter(checker)))
            {
                Assert.NotNull(producer);
            }
        }
    }
}
