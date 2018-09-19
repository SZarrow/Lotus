using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("TxnMsgContent")]
    public class TxnMsgRequestContent
    {
        //[XElement("txnType")]
        //public String TxnType { get; set; }
        //[XElement("interactiveStatus")]
        //public String InteractiveStatus { get; set; }
        //[XElement("entryTime")]
        //public String EntryTime { get; set; }
        //[XElement("externalRefNumber")]
        //public String ExternalRefNumber { get; set; }
        //[XElement("amount")]
        //public String Amount { get; set; }
        //[XElement("spFlag")]
        //public String SpFlag { get; set; }
        //[XElement("customerId")]
        //public String CustomerId { get; set; }
        //[XElement("payToken")]
        //public String PayToken { get; set; }
        //[XElement("tr3Url")]
        //public String NotifyUrl { get; set; }
        [XElement("extMap")]
        public ExtMap ExtMap { get; set; }
    }
}
