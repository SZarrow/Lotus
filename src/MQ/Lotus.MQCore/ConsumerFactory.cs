using System;
using System.Collections.Generic;
using System.Text;
using Lotus.MQCore.Common;

namespace Lotus.MQCore
{
    public abstract class ConsumerFactory
    {
        public abstract IConsumer CreateConsumer(String consumerId, SubscribeType subscribeType, params Object[] parameters);
    }
}
