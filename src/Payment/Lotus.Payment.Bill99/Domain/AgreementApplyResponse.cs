using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementApplyResponse : MasMessage
    {
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        [XElement("terminalId")]
        public String TerminalId { get; set; }
        [XElement("customerId")]
        public String CustomerId { get; set; }
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        [XElement("storablePan")]
        public String StorablePan { get; set; }
        [XElement("paytoken")]
        public String Paytoken { get; set; }
        [XElement("token")]
        public String Token { get; set; }
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
    }
}
