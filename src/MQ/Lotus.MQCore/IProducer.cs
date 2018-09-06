using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;
using Lotus.MQCore.Common;

namespace Lotus.MQCore
{
    public interface IProducer : IDisposable
    {
        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        XResult<Boolean> SendMessage(MQMessage message, params Object[] parameters);
        /// <summary>
        /// 发送顺序消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        XResult<Boolean> SendOrderMessage(MQMessage message, params Object[] parameters);
        /// <summary>
        /// 发送事务消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="parameters"></param>
        XResult<Boolean> SendTransactionMessage(MQMessage message, params Object[] parameters);
    }
}
