using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Serialization;

namespace Lotus.Payment.Bill99.Domain
{
    public class EntrustQueryResponse : MasMessage
    {
        [XElement("QryTxnMsgContent")]
        public EntrustQueryQryTxnMsgContent EntrustQueryQryTxnMsgContent { get; set; }
        [XElement("TxnMsgContent")]
        public EntrustQueryTxnMsgContent EntrustQueryTxnMsgContent { get; set; }
    }
}
