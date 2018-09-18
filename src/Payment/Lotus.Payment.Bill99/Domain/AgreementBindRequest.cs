using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementBindRequest : MasMessage
    {
        [XElement("indAuthDynVerifyContent")]
        public IndAuthDynVerifyRequestContent IndAuthDynVerifyContent { get; set; }
    }
}
