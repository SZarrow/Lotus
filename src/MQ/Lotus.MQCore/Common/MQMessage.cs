using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;

namespace Lotus.MQCore.Common
{
    public class MQMessage : IValidator
    {
        public String PublishTopics { get; set; }
        public String Tags { get; set; }
        public String MessageContent { get; set; }

        public XResult<Boolean> Validate()
        {
            if (String.IsNullOrWhiteSpace(this.PublishTopics))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("MQMessage.PublishTopics"));
            }

            if (String.IsNullOrWhiteSpace(this.Tags))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("MQMessage.Tags"));
            }

            if (String.IsNullOrWhiteSpace(this.MessageContent))
            {
                return new XResult<Boolean>(false, new ArgumentNullException("MQMessage.MessageContent"));
            }

            return new XResult<Boolean>(true);
        }
    }
}
