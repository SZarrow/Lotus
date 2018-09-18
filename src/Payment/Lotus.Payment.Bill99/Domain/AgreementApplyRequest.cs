using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementApplyRequest : MasMessage
    {
        [XElement("indAuthContent")]
        public IndAuthRequestContent IndAuthContent { get; set; }
    }
}
