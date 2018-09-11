using System;
using System.Collections.Generic;
using System.Text;
using DotNetWheels.Core;
using Lotus.Payment.Bill99.Domain;

namespace Lotus.Payment.Bill99
{
    /// <summary>
    /// 协议支付API
    /// </summary>
    public static class AgreementPaymentApi
    {
        /// <summary>
        /// 签约申请
        /// </summary>
        public static XResult<AgreementApplyResult> AgreementApply(AgreementApplyRequest request)
        {
            return new XResult<AgreementApplyResult>(null);
        }
    }
}
