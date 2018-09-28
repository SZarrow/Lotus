using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("QryTxnMsgContent")]
    public class QryTxnMsgRequestContent
    {
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("txnType")]
        public String TxnType { get; set; }
    }
}
