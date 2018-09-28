using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    [XElement("indAuthContent")]
    public class IndAuthRequestContent
    {
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
        /// 姓名
        /// </summary>
        [XElement("cardHolderName")]
        public String CardHolderName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [XElement("cardHolderId")]
        public String CardHolderId { get; set; }

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
