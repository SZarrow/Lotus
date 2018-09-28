using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("extDate")]
    public class ExtDate
    {
        [XElement("key")]
        public String Key { get; set; }
        [XElement("value")]
        public String Value { get; set; }
    }
}
