using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
    /// 委托代付API
    /// </summary>
    public class EntrustPaymentApi
    {
        private readonly HttpX _httpX;
        private readonly XSerializer _serializer;
        private const String NS = "http://www.99bill.com/mas_cnp_merchant_interface";

        public EntrustPaymentApi(HttpClient client, String merchantId, String terminalId)
        {
            if (String.IsNullOrWhiteSpace(merchantId))
            {
                throw new ArgumentNullException(nameof(merchantId));
            }

            if (String.IsNullOrWhiteSpace(terminalId))
            {
                throw new ArgumentNullException(nameof(terminalId));
            }

            _httpX = new HttpX(client);
            _serializer = new XSerializer();

            this.MerchantId = merchantId;
            this.TerminalId = terminalId;
        }


        public String MerchantId { get; }
        public String TerminalId { get; }

        public XResult<EntrustPayResponse> EntrustPay(String requestUrl, EntrustPayRequest request)
        {
            if (String.IsNullOrWhiteSpace(requestUrl))
            {
                return new XResult<EntrustPayResponse>(null, new ArgumentNullException(nameof(requestUrl)));
            }

            if (request == null)
            {
                return new XResult<EntrustPayResponse>(null, new ArgumentNullException(nameof(request)));
            }

            String xml = _serializer.Serialize(request, doc =>
            {
                var txnMsgContentEl = doc.Root.Element(XName.Get("TxnMsgContent", doc.Root.Name.NamespaceName));
                if (txnMsgContentEl != null)
                {
                    var terminalIdEl = new XElement("terminalId", this.TerminalId);
                    if (!String.IsNullOrWhiteSpace(txnMsgContentEl.Name.NamespaceName))
                    {
                        terminalIdEl.Name = XName.Get(terminalIdEl.Name.LocalName, txnMsgContentEl.Name.NamespaceName);
                    }
                    txnMsgContentEl.AddFirst(terminalIdEl);

                    var merchantIdEl = new XElement("merchantId", this.MerchantId);
                    if (!String.IsNullOrWhiteSpace(txnMsgContentEl.Name.NamespaceName))
                    {
                        merchantIdEl.Name = XName.Get(merchantIdEl.Name.LocalName, txnMsgContentEl.Name.NamespaceName);
                    }
                    txnMsgContentEl.AddFirst(merchantIdEl);
                }
            });

            WriteLog("EntrustPayRequestData：" + xml);

            XResult<EntrustPayResponse> result = null;

            var task = _httpX.PostXmlAsync<EntrustPayResponse>(requestUrl, xml).ContinueWith(t0 =>
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
                return new XResult<EntrustPayResponse>(null, ex);
            }
        }

        /// <summary>
        /// 查询交易流水
        /// </summary>
        /// <param name="requestUrl">请求地址</param>
        /// <param name="request">请求内容</param>
        public XResult<EntrustQueryResponse> EntrustQuery(String requestUrl, EntrustQueryRequest request)
        {
            if (String.IsNullOrWhiteSpace(requestUrl))
            {
                return new XResult<EntrustQueryResponse>(null, new ArgumentNullException(nameof(requestUrl)));
            }

            if (request == null)
            {
                return new XResult<EntrustQueryResponse>(null, new ArgumentNullException(nameof(request)));
            }

            String xml = _serializer.Serialize(request, doc =>
            {
                var qryTxnMsgContentEl = doc.Root.Element(XName.Get("QryTxnMsgContent", doc.Root.Name.NamespaceName));
                if (qryTxnMsgContentEl != null)
                {
                    var terminalIdEl = new XElement("terminalId", this.TerminalId);
                    if (!String.IsNullOrWhiteSpace(qryTxnMsgContentEl.Name.NamespaceName))
                    {
                        terminalIdEl.Name = XName.Get(terminalIdEl.Name.LocalName, qryTxnMsgContentEl.Name.NamespaceName);
                    }
                    qryTxnMsgContentEl.AddFirst(terminalIdEl);

                    var merchantIdEl = new XElement("merchantId", this.MerchantId);
                    if (!String.IsNullOrWhiteSpace(qryTxnMsgContentEl.Name.NamespaceName))
                    {
                        merchantIdEl.Name = XName.Get(merchantIdEl.Name.LocalName, qryTxnMsgContentEl.Name.NamespaceName);
                    }
                    qryTxnMsgContentEl.AddFirst(merchantIdEl);
                }
            });

            XResult<EntrustQueryResponse> result = null;

            var task = _httpX.PostXmlAsync<EntrustQueryResponse>(requestUrl, xml).ContinueWith(t0 =>
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
                return new XResult<EntrustQueryResponse>(null, ex);
            }
        }

        private void WriteLog(String content)
        {
            String logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $@"log\{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            File.AppendAllText(logFile, Environment.NewLine + Environment.NewLine + content);
        }
    }
}
