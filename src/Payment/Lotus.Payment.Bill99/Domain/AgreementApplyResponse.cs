using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementApplyResponse : MasMessage
    {
        public String MerchantId { get; set; }
        public String TerminalId { get; set; }
        public String CustomerId { get; set; }
        public String ExternalRefNumber { get; set; }
        public String StorablePan { get; set; }
        public String Paytoken { get; set; }
        public String Token { get; set; }
        public String ResponseCode { get; set; }
        public String ResponseTextMessage { get; set; }
    }
}
