using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core.Extensions;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustPayAccount
    {
        /// <summary>
        /// 收款用途，对应合同中约定业务
        /// </summary>
        public String Usage { get; set; }
        /// <summary>
        /// 收款金额>0。单位为：元。保留到小数点后两位。
        /// </summary>
        public Decimal Amount { get; set; }
        /// <summary>
        /// 商家订单号，该订单对应在商家系统中的流水号，在一个批次中唯一即可(对于批量而言)
        /// </summary>
        public String SeqId { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public String BankId { get; set; }
        /// <summary>
        /// 付款账户类型
        /// </summary>
        public AccountType AccountType { get; set; }
        /// <summary>
        /// 付款方姓名
        /// </summary>
        public String BankAcctName { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public String IdCode { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public String BankAcctId { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public String CurrencyType
        {
            get
            {
                return "CNY";
            }
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            DateTime now = DateTime.Now;

            sb.Append("<tns:items>");
            sb.Append("<tns:seqId>" + now.ToString("yyyyMMddHHmmss") + "</tns:seqId>");
            sb.Append($"<tns:usage>{this.Usage}</tns:usage>");
            sb.Append($"<tns:bankId>{this.BankId}</tns:bankId>");
            sb.Append($"<tns:accType>{this.AccountType.GetValue()}</tns:accType>");
            sb.Append($"<tns:bankAcctName>{this.BankAcctName}</tns:bankAcctName>");
            sb.Append($"<tns:bankAcctId>{this.BankAcctId}</tns:bankAcctId>");
            sb.Append("<tns:idType>101</tns:idType>");
            sb.Append($"<tns:idCode>{this.IdCode}</tns:idCode>");
            sb.Append($"<tns:amount>{this.Amount}</tns:amount>");
            sb.Append($"<tns:curType>{this.CurrencyType}</tns:curType>");
            sb.Append("</tns:items>");

            return sb.ToString();
        }
    }
}
