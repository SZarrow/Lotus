using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("QryTxnMsgContent")]
    public class EntrustQueryQryTxnMsgContent
    {
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("txnType")]
        public String TxnType { get; set; }
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("terminalId")]
        public String TerminalId { get; set; }
    }
}
