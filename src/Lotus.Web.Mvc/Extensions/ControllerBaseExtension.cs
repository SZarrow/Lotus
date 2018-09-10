using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtension
    {
        public static JsonResult FormatJson(this ControllerBase controller, Object value, Int32 errCode, String errMsg)
        {
            return new JsonResult(new { ErrCode = errCode, ErrMsg = errMsg, Value = value });
        }

        public static JsonResult FormatJson(this ControllerBase controller, Object value)
        {
            return new JsonResult(new { ErrCode = 0, Value = value });
        }
    }
}
