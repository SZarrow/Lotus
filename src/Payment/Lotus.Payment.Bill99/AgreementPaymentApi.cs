using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lotus.Core;
using Lotus.Net.Http;
using Lotus.Payment.Bill99.Domain;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99
{
    /// <summary>
    /// 协议支付API
    /// </summary>
    public class AgreementPaymentApi
    {
        private readonly HttpX _httpX;
        private readonly XSerializer _serializer;
        private const String NS = "http://www.99bill.com/mas_cnp_merchant_interface";

        public AgreementPaymentApi(HttpClient client, String merchantId, String terminalId)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

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

            _httpX = new HttpX(client);
            _serializer = new XSerializer();
        }

        public String MerchantId { get; }
        public String TerminalId { get; }

        /// <summary>
        /// 签约申请
        /// </summary>
        public XResult<AgreementApplyResponse> AgreementApply(String requestUrl, AgreementApplyRequest request)
        {
            if (String.IsNullOrWhiteSpace(requestUrl))
            {
                return new XResult<AgreementApplyResponse>(null, new ArgumentNullException(nameof(requestUrl)));
            }

            if (request == null)
            {
                return new XResult<AgreementApplyResponse>(null, new ArgumentNullException(nameof(request)));
            }

            request.MerchantId = this.MerchantId;
            request.TerminalId = this.TerminalId;

            String xml = _serializer.Serialize(request);

            XResult<AgreementApplyResponse> result = null;

            var task = _httpX.PostXmlAsync<AgreementApplyResponse>(requestUrl, xml).ContinueWith(t0 =>
            {
                if (t0.IsCompleted)
                {
                    if (t0.IsCanceled || t0.IsFaulted)
                    {
                        throw new TaskCanceledException();
                    }

                    result = t0.Result;
                }
            });

            try
            {
                task.Wait();
                return result;
            }
            catch (Exception ex)
            {
                return new XResult<AgreementApplyResponse>(null, ex);
            }
        }
    }
}
