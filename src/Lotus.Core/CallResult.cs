using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Core
{
    [Serializable]
    public class CallResult<T> : XResult<T>
    {
        public CallResult(T value) : base(value)
        {
            this.ErrCode = 0;
            this.ErrMsg = String.Empty;
        }

        public CallResult(T value, Int32 errCode, Exception ex) : base(value, ex)
        {
            this.ErrCode = errCode;
            this.ErrMsg = ex != null ? ex.Message : String.Empty;
        }

        public Int32 ErrCode { get; }
        public String ErrMsg { get; }
    }
}
