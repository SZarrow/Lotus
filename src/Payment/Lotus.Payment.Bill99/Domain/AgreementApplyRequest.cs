using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DotNetWheels.Core;

namespace Lotus.Payment.Bill99.Domain
{
    public class AgreementApplyRequest : XmlSerializer<AgreementApplyRequest>
    {
        private const String NS = "http://www.99bill.com/mas_cnp_merchant_interface";

        public String MerchantId { get; set; }
        public String TerminalId { get; set; }
        public String CustomerId { get; set; }
        public String ExternalRefNumber { get; set; }
        public String Pan { get; set; }
        public String PhoneNO { get; set; }
        public String BindType { get; set; }

        public override void BuildDocument(XDocument doc)
        {
            doc.Add(new XElement(XName.Get("MasMessage", NS)));
            doc.Root.Add(new XElement(XName.Get("version", NS), "1.0"));
            var contentEl = new XElement(XName.Get("indAuthContent", NS));

            var properties = typeof(AgreementApplyRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                contentEl.Add(new XElement(XName.Get(GetCamelName(prop.Name), NS), prop.XGetValue(this)));
            }

            doc.Root.Add(contentEl);
        }
    }
}
