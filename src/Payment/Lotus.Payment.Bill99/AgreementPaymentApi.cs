using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DotNetWheels.Core;
using Lotus.Net.Http;
using Lotus.Payment.Bill99.Domain;

namespace Lotus.Payment.Bill99
{
    /// <summary>
    /// 协议支付API
    /// </summary>
    public class AgreementPaymentApi
    {
        private readonly XmlSerializer _serializer;
        private const String NS = "http://www.99bill.com/mas_cnp_merchant_interface";

        public AgreementPaymentApi(String merchantId, String terminalId)
        {
            if (String.IsNullOrWhiteSpace(merchantId))
            {
                throw new ArgumentNullException(nameof(merchantId));
            }

            if (String.IsNullOrWhiteSpace(terminalId))
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            this.MerchantId = merchantId;
            this.TerminalId = terminalId;

            _serializer = new XmlSerializer();
        }

        public String MerchantId { get; }
        public String TerminalId { get; }

        /// <summary>
        /// 签约申请
        /// </summary>
        public async Task<XResult<AgreementApplyResult>> AgreementApply(AgreementApplyRequest request)
        {
            String xml = _serializer.ToXml(doc =>
            {
                doc.Add(new XElement(XName.Get("MasMessage", NS)));
                doc.Root.Add(new XElement(XName.Get("version", NS), "1.0"));
                var contentEl = new XElement(XName.Get("indAuthContent", NS));

                contentEl.Add(new XElement(XName.Get(_serializer.GetCamelName("MerchantId"), NS), this.MerchantId));
                contentEl.Add(new XElement(XName.Get(_serializer.GetCamelName("TerminalId"), NS), this.TerminalId));

                var properties = typeof(AgreementApplyRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    contentEl.Add(new XElement(XName.Get(_serializer.GetCamelName(prop.Name), NS), prop.XGetValue(request)));
                }

                doc.Root.Add(contentEl);
            });

            HttpX httpX = new HttpX(null);

            var result = await httpX.PostXmlAsync<AgreementApplyResult>(request.RequestUrl, xml);

            return new XResult<AgreementApplyResult>(null);
        }
    }
}
