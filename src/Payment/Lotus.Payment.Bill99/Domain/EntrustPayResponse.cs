using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustPayResponse
    {
        [XElement("merchantAcctId")]
        public String MemberCode { get; set; }
        [XElement("contractId")]
        public String ContractId { get; set; }
        [XElement("requestId")]
        public String RequestId { get; set; }
        [XElement("requestTime")]
        public String RequestTime { get; set; }
        [XElement("numTotal")]
        public Int32 NumTotal { get; set; }
        [XElement("amountTotal")]
        public Int32 AmountTotal { get; set; }
        [XElement("receiveTime")]
        public DateTime ReceiveTime { get; set; }
        [XElement("batchId")]
        public String BatchId { get; set; }
        [XElement("dealResult")]
        public String DealResult { get; set; }
        [XElement("batchErr")]
        public String BatchErr { get; set; }
        [XElement("batchErrMessage")]
        public String BatchErrMessage { get; set; }
        [XElement("ext1")]
        public String Ext1 { get; set; }
        [XElement("ext2")]
        public String Ext2 { get; set; }
    }
}
