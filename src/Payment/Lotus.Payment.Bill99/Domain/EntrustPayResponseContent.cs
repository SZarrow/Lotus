using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("TxnMsgContent")]
    public class EntrustPayResponseContent
    {
        [XElement("txnType")]
        public String TxnType { get; set; }
        [XElement("interactiveStatus")]
        public String InteractiveStatus { get; set; }
        [XElement("amount")]
        public String Amount { get; set; }
        [XElement("entryTime")]
        public String EntryTime { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("transTime")]
        public String TransTime { get; set; }
        [XElement("refNumber")]
        public String RefNumber { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
        [XElement("cardOrg")]
        public String CardOrg { get; set; }
        [XElement("issuer")]
        public String Issuer { get; set; }
        [XElement("storableCardNo")]
        public String StorableCardNo { get; set; }
        [XElement("authorizationCode")]
        public String AuthorizationCode { get; set; }
    }
}
