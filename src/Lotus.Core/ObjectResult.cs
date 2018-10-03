using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Core
{
    [Serializable]
    public class ObjectResult : XResult<Object>
    {
        public ObjectResult(Object value, params Exception[] exceptions) : base(value, exceptions) { }
    }
}
