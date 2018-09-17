using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [Serializable]
    [XElement("indAuthContent")]
    public class AgreementApplyRequest : MasMessage
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
        /// 银行卡号
        /// </summary>
        [XElement("pan")]
        public String Pan { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [XElement("phoneNO")]
        public String PhoneNO { get; set; }
        /// <summary>
        /// 接入方式，0表示商户绑定
        /// </summary>
        [XElement("bindType")]
        public String BindType { get; set; }
    }
}
