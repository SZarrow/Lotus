using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("indAuthDynVerifyContent")]
    public class IndAuthDynVerifyResponseContent
    {
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("customerId")]
        public String CustomerId { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("storablePan")]
        public String StorablePan { get; set; }
        [XElement("payToken")]
        public String PayToken { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
    }
}
