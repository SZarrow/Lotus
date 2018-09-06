using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Lotus.MQCore.Common;
using Xunit;

namespace Lotus.MQProvider.RocketMQ.Tests
{
    public class ConsumerFactoryTest
    {
        [Fact]
        public void TestCreateClusterConsumer()
        {
            Thread.Sleep(10);
            var fac = new RocketMQConsumerFactory();
            var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.CLUSTERING);
            Assert.NotNull(consumer);
        }

        [Fact]
        public void TestCreateBroadcastConsumer()
        {
            Thread.Sleep(10);
            var fac = new RocketMQConsumerFactory();
            var consumer = fac.CreateConsumer("CID_sxb_01", SubscribeType.BROADCASTING);
            Assert.NotNull(consumer);
        }
    }
}
