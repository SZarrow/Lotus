using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementQueryResponse : MasMessage
    {
        [XElement("ErrorMsgContent")]
        public ErrorMsgContent ErrorMsgContent { get; set; }

        [XElement("TxnMsgContent")]
        public QryTxnMsgResponseContent QryTxnMsgContent { get; set; }
    }
}
