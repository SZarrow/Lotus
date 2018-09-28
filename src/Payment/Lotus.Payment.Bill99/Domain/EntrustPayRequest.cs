using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustPayRequest : MasMessage
    {
        [XElement("TxnMsgContent")]
        public EntrustPayRequestContent EntrustPayRequestContent { get; set; }
    }
}
