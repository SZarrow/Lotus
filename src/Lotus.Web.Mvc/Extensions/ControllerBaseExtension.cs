using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtension
    {
        public static JsonResult Failure(this ControllerBase controller, Object value, String errMsg)
        {
            return new JsonResult(new { Status = "FAILURE", ErrMsg = errMsg, Value = value });
        }

        public static JsonResult Success(this ControllerBase controller, Object value)
        {
            return new JsonResult(new { Status = "SUCCESS", Value = value });
        }

        public static JsonResult Json(this ControllerBase controller, String status, Object value, String errMsg = null)
        {
            if (String.IsNullOrWhiteSpace(errMsg))
            {
                return new JsonResult(new { Status = status, Value = value });
            }
            else
            {
                return new JsonResult(new { Status = status, ErrMsg = errMsg, Value = value });
            }
        }
    }
}
