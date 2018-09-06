using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lotus.MQCore.Common;
using Xunit;

namespace Lotus.MQProvider.RocketMQ.Tests
{
    public class ProducerTest
    {
        [Fact]
        public void TestSendMessage()
        {
            Thread.Sleep(10);
            var fac = new RocketMQProducerFactory();
            using (var prod = fac.CreateProducer("PID_sxb_01", MQMessageType.GeneralMessage))
            {
                var result = prod.SendMessage(new MQMessage()
                {
                    PublishTopics = "sxb_test_01",
                    Tags = "TestSendMessage",
                    MessageContent = $"Hello, TestSendMessage() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                });
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void TestSendOrderMessage()
        {
            Thread.Sleep(10);
            var fac = new RocketMQProducerFactory();
            using (var prod = fac.CreateProducer("PID_sxb_01", MQMessageType.OrderMessage))
            {
                var result = prod.SendOrderMessage(new MQMessage()
                {
                    PublishTopics = "sxb_test_01",
                    Tags = "TestSendOrderMessage",
                    MessageContent = $"Hello, TestSendOrderMessage() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                });
                Assert.True(result.Success);
            }
        }

        [Fact]
        public void TestSendTransactionMessage()
        {
            Thread.Sleep(10);
            var fac = new RocketMQProducerFactory();
            using (var prod = fac.CreateProducer("PID_sxb_01", MQMessageType.TransactionMessage, RocketMQProducerFactory.CreateTransactionMessageParameter(msg =>
             {
                 return ons.TransactionStatus.CommitTransaction;
             })))
            {
                var result = prod.SendTransactionMessage(new MQMessage()
                {
                    PublishTopics = "sxb_test_01",
                    Tags = "TestSendTransactionMessage",
                    MessageContent = $"Hello, TestSendTransactionMessage() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                }, RocketMQProducer.CreateTransactionMessageParameter(msg =>
                {
                    return ons.TransactionStatus.CommitTransaction;
                }));
                Assert.True(result.Success);
            }
        }
    }
}
