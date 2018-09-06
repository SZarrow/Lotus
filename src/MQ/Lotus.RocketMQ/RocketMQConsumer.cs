using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;
using Lotus.MQCore;
using ons;

namespace Lotus.MQProvider.RocketMQ
{
    public class RocketMQConsumer : IConsumer
    {
        private PushConsumer _pushConsumer;
        private Boolean _isStarted = false;

        public RocketMQConsumer(PushConsumer consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException(nameof(consumer));
            }

            _pushConsumer = consumer;
        }

        public XResult<Boolean> Subscribe(String publishedTopics, String tags, params Object[] parameters)
        {
            if (String.IsNullOrWhiteSpace(publishedTopics))
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(publishedTopics)));
            }

            if (String.IsNullOrWhiteSpace(tags))
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(tags)));
            }

            if (_pushConsumer == null)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(_pushConsumer)));
            }

            if (parameters == null || parameters.Length == 0)
            {
                return new XResult<Boolean>(false, new ArgumentNullException(nameof(parameters)));
            }

            var listener = parameters[0] as MessageListener;
            if (listener == null)
            {
                return new XResult<Boolean>(false, new FormatException("parameters[0] is not MessageListener"));
            }

            try
            {
                _pushConsumer.subscribe(publishedTopics, tags, listener);
                return Start();
            }
            catch (Exception ex)
            {
                return new XResult<Boolean>(false, ex);
            }
        }

        public static Object CreateSubscribeParameter(Func<Message, ConsumeContext, ons.Action> consume)
        {
            if (consume == null)
            {
                throw new ArgumentNullException(nameof(consume));
            }

            return new RocketMQMessageListener(consume);
        }

        public void Dispose()
        {
            if (_pushConsumer != null)
            {
                try
                {
                    _pushConsumer.shutdown();
                    _pushConsumer.Dispose();
                    _isStarted = false;
                }
                catch { }
            }
        }

        private XResult<Boolean> Start()
        {
            if (!_isStarted)
            {
                try
                {
                    _pushConsumer.start();
                    _isStarted = true;
                }
                catch (Exception ex)
                {
                    return new XResult<Boolean>(false, ex);
                }
            }

            return new XResult<Boolean>(true);
        }
    }

    internal class RocketMQMessageListener : MessageListener
    {
        private Func<Message, ConsumeContext, ons.Action> _consume;

        public RocketMQMessageListener(Func<Message, ConsumeContext, ons.Action> consume)
        {
            if (consume == null)
            {
                throw new ArgumentNullException(nameof(consume));
            }

            _consume = consume;
        }

        public override ons.Action consume(Message message, ConsumeContext context)
        {
            return _consume(message, context);
        }
    }
}
