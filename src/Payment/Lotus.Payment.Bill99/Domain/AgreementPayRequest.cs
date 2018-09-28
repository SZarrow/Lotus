using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementPayRequest : MasMessage
    {
        [XElement("TxnMsgContent")]
        public TxnMsgRequestContent TxnMsgContent { get; set; }
    }
}
