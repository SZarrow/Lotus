using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Core;

namespace Lotus.MQCore.Common
{
    public interface IValidator
    {
        XResult<Boolean> Validate();
    }
}
