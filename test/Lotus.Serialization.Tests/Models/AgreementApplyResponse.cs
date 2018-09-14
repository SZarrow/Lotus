using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Lotus.Serialization.Tests.Models
{
    /// <summary>
    /// 签约申请返回消息
    /// </summary>
    public class AgreementApplyResponse : MasMessage
    {
        /// <summary>
        /// 商户Id
        /// </summary>
        [XElement("merchantId")]
        public String MerchantId { get; set; }
        /// <summary>
        /// 终端Id
        /// </summary>
        [XElement("terminalId")]
        public String TerminalId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [XElement("customerId")]
        public String CustomerId { get; set; }
        /// <summary>
        /// 外部跟踪编号
        /// </summary>
        [XElement("externalRefNumber")]
        public String ExternalRefNumber { get; set; }
        /// <summary>
        /// 缩略卡号
        /// </summary>
        [XElement("storablePan")]
        public String StorablePan { get; set; }
        /// <summary>
        /// 支付标记
        /// </summary>
        [XElement("paytoken")]
        public String Paytoken { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        [XElement("token")]
        public String Token { get; set; }
        /// <summary>
        /// 应答码
        /// </summary>
        [XElement("responseCode")]
        public String ResponseCode { get; set; }
        /// <summary>
        /// 应答码文本消息
        /// </summary>
        [XElement("responseTextMessage")]
        public String ResponseTextMessage { get; set; }
    }
}
