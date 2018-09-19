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
        /// <param name="requestUrl">请求地址</param>
        /// <param name="request">请求内容</param>
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

            String xml = _serializer.Serialize(request, doc =>
            {
                var indAuthContentEl = doc.Root.Element(XName.Get("indAuthContent", doc.Root.Name.NamespaceName));
                if (indAuthContentEl != null)
                {
                    var terminalIdEl = new XElement("terminalId", this.TerminalId);
                    if (!String.IsNullOrWhiteSpace(indAuthContentEl.Name.NamespaceName))
                    {
                        terminalIdEl.Name = XName.Get(terminalIdEl.Name.LocalName, indAuthContentEl.Name.NamespaceName);
                    }
                    indAuthContentEl.AddFirst(terminalIdEl);

                    var merchantIdEl = new XElement("merchantId", this.MerchantId);
                    if (!String.IsNullOrWhiteSpace(indAuthContentEl.Name.NamespaceName))
                    {
                        merchantIdEl.Name = XName.Get(merchantIdEl.Name.LocalName, indAuthContentEl.Name.NamespaceName);
                    }
                    indAuthContentEl.AddFirst(merchantIdEl);
                }
            });

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

        /// <summary>
        /// 签约验证
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="request">请求内容</param>
        public XResult<AgreementBindResponse> AgreementVerify(String requestUrl, AgreementBindRequest request)
        {
            if (String.IsNullOrWhiteSpace(requestUrl))
            {
                return new XResult<AgreementBindResponse>(null, new ArgumentNullException(nameof(requestUrl)));
            }

            if (request == null)
            {
                return new XResult<AgreementBindResponse>(null, new ArgumentNullException(nameof(request)));
            }

            String xml = _serializer.Serialize(request, doc =>
            {
                var indAuthDynVerifyContentEl = doc.Root.Element(XName.Get("indAuthDynVerifyContent", doc.Root.Name.NamespaceName));
                if (indAuthDynVerifyContentEl != null)
                {
                    var merchantIdEl = new XElement("merchantId", this.MerchantId);
                    if (!String.IsNullOrWhiteSpace(indAuthDynVerifyContentEl.Name.NamespaceName))
                    {
                        merchantIdEl.Name = XName.Get(merchantIdEl.Name.LocalName, indAuthDynVerifyContentEl.Name.NamespaceName);
                    }
                    indAuthDynVerifyContentEl.AddFirst(merchantIdEl);
                }
            });

            XResult<AgreementBindResponse> result = null;

            var task = _httpX.PostXmlAsync<AgreementBindResponse>(requestUrl, xml).ContinueWith(t0 =>
            {
                if (t0.IsCompleted)
                {
                    if (t0.IsCanceled || t0.IsFaulted)
                    {
                        throw new TaskCanceledException($"RequestUrl:{requestUrl},Content:{xml}");
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
                return new XResult<AgreementBindResponse>(null, ex);
            }
        }

        public XResult<AgreementPayResponse> AgreementPay(String requestUrl, AgreementPayRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
