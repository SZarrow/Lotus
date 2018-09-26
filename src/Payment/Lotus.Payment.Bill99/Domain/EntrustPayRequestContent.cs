using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core.Extensions;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("TxnMsgContent")]
    public class EntrustPayRequestContent
    {
        [XElement("interactiveStatus")]
        public String InteractiveStatus { get; set; }
        [XElement("txnType")]
        public String TxnType { get; set; }
        [XElement("entryTime")]
        public String EntryTime { get; set; }
        [XElement("cardNo")]
        public String CardNo { get; set; }
        [XElement("amount")]
        public String Amount { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("cardHolderName")]
        public String CardHolderName { get; set; }
        [XElement("idType")]
        public String IdType { get; set; }
        [XElement("cardHolderId")]
        public String CardHolderId { get; set; }
        [XElement("extMap")]
        public ExtMap ExtMap { get; set; }
    }
}
