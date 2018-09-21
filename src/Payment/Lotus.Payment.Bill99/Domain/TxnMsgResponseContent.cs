using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("TxnMsgContent")]
    public class TxnMsgResponseContent
    {
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("terminalId")]
        public String TerminalId { get; set; }
        [XElement("txnType")]
        public String TxnType { get; set; }
        [XElement("interactiveStatus")]
        public String InteractiveStatus { get; set; }
        [XElement("entryTime")]
        public String EntryTime { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("amount")]
        public String Amount { get; set; }
        [XElement("customerId")]
        public String CustomerId { get; set; }
        [XElement("transTime")]
        public String TransTime { get; set; }
        [XElement("payToken")]
        public String PayToken { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
    }
}
