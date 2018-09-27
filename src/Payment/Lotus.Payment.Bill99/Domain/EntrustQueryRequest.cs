using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustQueryRequest : MasMessage
    {
        [XElement("QryTxnMsgContent")]
        public EntrustQueryRequestContent EntrustQueryRequestContent { get; set; }
    }
}
