using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("indAuthDynVerifyContent")]
    public class IndAuthDynVerifyRequestContent
    {
        [XElement("customerId")]
        public String CustomerId { get; set; }

        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }

        [XElement("pan")]
        public String Pan { get; set; }

        [XElement("phoneNO")]
        public String PhoneNO { get; set; }

        [XElement("validCode")]
        public String ValidCode { get; set; }

        [XElement("token")]
        public String Token { get; set; }

        [XElement("bindType")]
        public String BindType { get; set; }
    }
}
