using System;
using System.Collections.Generic;
using System.Text;
using Lotus.MQCore.Common;

namespace Lotus.MQCore
{
    public abstract class ProducerFactory
    {
        public abstract IProducer CreateProducer(String producerId, MQMessageType messageType, params Object[] parameters);
    }
}
