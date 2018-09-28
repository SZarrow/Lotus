using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("extMap")]
    public class ExtMap
    {
        [XCollection]
        public IEnumerable<ExtDate> ExtDates { get; set; }
    }
}
