using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementQueryRequest : MasMessage
    {
        [XElement("QryTxnMsgContent")]
        public QryTxnMsgRequestContent QryTxnMsgContent { get; set; }
    }
}
