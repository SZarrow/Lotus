using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Serialization.Tests.Models
{
    [XElement("MasMessage", "http://www.99bill.com/mas_cnp_merchant_interface")]
    public abstract class MasMessage
    {
        protected MasMessage() { }

        [XElement("version")]
        public String Version { get; set; }
    }
}
