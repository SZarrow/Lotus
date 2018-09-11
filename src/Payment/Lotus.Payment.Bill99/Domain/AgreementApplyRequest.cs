using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DotNetWheels.Core;

namespace Lotus.Payment.Bill99.Domain
{
    [Serializable]
    public class AgreementApplyRequest
    {
        /// <summary>
        /// 请求的Url
        /// </summary>
        public String RequestUrl { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public String CustomerId { get; set; }
        /// <summary>
        /// 外部跟踪编号
        /// </summary>
        public String ExternalRefNumber { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public String Pan { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public String PhoneNO { get; set; }
        /// <summary>
        /// 接入方式，0表示商户绑定
        /// </summary>
        public String BindType { get; set; }
    }
}
