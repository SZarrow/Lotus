using System;
using System.Collections.Generic;
using System.Text;

namespace Lotus.Core
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute
    {
        public EnumValueAttribute(Object value)
        {
            this.Value = value;
        }

        public Object Value { get; }
    }
}
