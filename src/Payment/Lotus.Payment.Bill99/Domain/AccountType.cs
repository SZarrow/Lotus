using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;

namespace Lotus.Payment.Bill99.Domain
{
    /// <summary>
    /// 付款账户类型
    /// </summary>
    public enum AccountType
    {
        [EnumValue("0100")]
        对公账户,
        [EnumValue("0101")]
        对公存款账户,
        [EnumValue("0200")]
        个人账户,
        [EnumValue("0201")]
        个人借记卡账户,
        [EnumValue("0204")]
        个人存折账户
    }
}
