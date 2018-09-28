using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustPayResponse:MasMessage
    {
        [XElement("TxnMsgContent")]
        public EntrustPayResponseContent EntrustPayResponseContent { get; set; }
    }
}
