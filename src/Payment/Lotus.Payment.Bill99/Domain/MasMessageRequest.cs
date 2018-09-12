using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("MasMessage", "http://www.99bill.com/mas_cnp_merchant_interface")]
    public abstract class MasMessageRequest
    {
        [XElement("version")]
        public String Version { get; set; }
    }
}
