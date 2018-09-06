using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lotus.MQCore.Common;
using Xunit;

namespace Lotus.MQProvider.RocketMQ.Tests
{
    public class ConsumerTest
    {
        [Fact]
        public void TestSubscribeClusterMessageWithSingleTag()
        {
            var fac = new RocketMQConsumerFactory();
            using (var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.CLUSTERING))
            {
                var result = consumer.Subscribe("sxb_test_01", "TestSendMessage", RocketMQConsumer.CreateSubscribeParameter((msg, context) =>
                 {
                     return ons.Action.CommitMessage;
                 }));

                var prodFac = new RocketMQProducerFactory();
                using (var producer = prodFac.CreateProducer("PID_sxb_01", MQMessageType.GeneralMessage))
                {
                    var x = producer.SendMessage(new MQMessage()
                    {
                        PublishTopics = "sxb_test_01",
                        Tags = "TestSendMessage",
                        MessageContent = $"Hello, TestSubscribeClusterMessageWithSingleTag() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                    });

                    Assert.True(x.Success);
                }

                using (var producer = prodFac.CreateProducer("PID_sxb_01", MQMessageType.OrderMessage))
                {
                    var x = producer.SendOrderMessage(new MQMessage()
                    {
                        PublishTopics = "sxb_test_01",
                        Tags = "TestSendOrderMessage",
                        MessageContent = $"Hello, TestSendOrderMessage() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                    });

                    Assert.True(x.Success);
                }

                using (var producer = prodFac.CreateProducer("PID_sxb_01", MQMessageType.TransactionMessage,
                    RocketMQProducerFactory.CreateTransactionMessageParameter(msg =>
                    {
                        return ons.TransactionStatus.CommitTransaction;
                    })))
                {
                    var x = producer.SendTransactionMessage(new MQMessage()
                    {
                        PublishTopics = "sxb_test_01",
                        Tags = "SendTransactionMessage",
                        MessageContent = $"Hello, SendTransactionMessage() at {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}"
                    }, RocketMQProducer.CreateTransactionMessageParameter(msg =>
                    {
                        return ons.TransactionStatus.CommitTransaction;
                    }));

                    Assert.True(x.Success);
                }

                Thread.Sleep(10 * 1000);

                Assert.True(result.Success);
            }
        }

        [Fact]
        public void TestSubscribeClusterMessageWithMultiTags()
        {
            var fac = new RocketMQConsumerFactory();
            using (var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.CLUSTERING))
            {
                var result = consumer.Subscribe("sxb_test_01", "TestSendTransactionMessage,TestSendMessage", RocketMQConsumer.CreateSubscribeParameter((msg, context) =>
                 {
                     return ons.Action.CommitMessage;
                 }));

                Thread.Sleep(10 * 1000);

                Assert.True(result.Success);
            }
        }
    }
}
