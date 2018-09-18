using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("ErrorMsgContent")]
    public class ErrorMsgContent
    {
        [XElement("errorCode")]
        public String ErrorCode { get; set; }

        [XElement("errorMessage")]
        public String ErrorMessage { get; set; }
    }
}
