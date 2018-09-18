using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementApplyResponse : MasMessage
    {
        [XElement("ErrorMsgContent")]
        public ErrorMsgContent ErrorMsgContent { get; set; }

        [XElement("indAuthContent")]
        public IndAuthResponseContent IndAuthContent { get; set; }
    }
}
