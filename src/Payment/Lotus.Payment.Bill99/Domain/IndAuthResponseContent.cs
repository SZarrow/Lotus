using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("indAuthContent")]
    public class IndAuthResponseContent
    {
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("terminalId")]
        public String TerminalId { get; set; }
        [XElement("customerId")]
        public String CustomerId { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("pan")]
        public String Pan { get; set; }
        [XElement("storablePan")]
        public String StorablePan { get; set; }
        [XElement("payToken")]
        public String PayToken { get; set; }
        [XElement("token")]
        public String Token { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
    }
}
