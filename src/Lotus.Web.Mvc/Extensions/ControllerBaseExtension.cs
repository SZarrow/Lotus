using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtension
    {
        public static JsonResult FormatJson(this ControllerBase controller, Object value, String errMsg)
        {
            return new JsonResult(new { Status = "FAILURE", ErrMsg = errMsg, Value = value });
        }

        public static JsonResult FormatJson(this ControllerBase controller, Object value)
        {
            return new JsonResult(new { Status = "SUCCESS", Value = value });
        }
    }
}
