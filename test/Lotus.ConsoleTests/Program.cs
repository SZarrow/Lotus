using System;
using Lotus.MQCore.Common;
using Lotus.MQProvider.RocketMQ;

namespace Lotus.ConsoleTests
{
    class Program
    {
        static void Main(String[] args)
        {
            TestSubscribeClusterMessageWithSingleTag();

            Console.WriteLine("完成");
            Console.ReadKey();
        }

        static void TestSubscribeClusterMessageWithSingleTag()
        {
            var fac = new RocketMQConsumerFactory();
            using (var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.CLUSTERING))
            {
                var result = consumer.Subscribe("sxb_test_01", "TestSendOrderMessage", RocketMQConsumer.CreateSubscribeParameter((msg, context) =>
                {
                    return ons.Action.CommitMessage;
                }));

                Console.ReadKey();
            }
        }

        static void TestSubscribeClusterMessageWithMultiTags()
        {
            var fac = new RocketMQConsumerFactory();
            using (var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.CLUSTERING))
            {
                var result = consumer.Subscribe("sxb_test_01", "TestSendTransactionMessage,TestSendMessage", RocketMQConsumer.CreateSubscribeParameter((msg, context) =>
                {
                    return ons.Action.CommitMessage;
                }));

                Console.ReadKey();
            }
        }
    }
}
