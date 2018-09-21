using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("TxnMsgContent")]
    public class QryTxnMsgResponseContent
    {
        [XElement("txnType")]
        public String TxnType { get; set; }
        [XElement("txnStatus")]
        public String TxnStatus { get; set; }
        [XElement("amount")]
        public String Amount { get; set; }
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("terminalId")]
        public String TerminalId { get; set; }
        [XElement("entryTime")]
        public String EntryTime { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("transTime")]
        public String TransTime { get; set; }
        [XElement("voidFlag")]
        public String VoidFlag { get; set; }
        [XElement("refNumber")]
        public String RefNumber { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
        [XElement("authorizationCode")]
        public String AuthorizationCode { get; set; }
    }
}
