using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Core
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public String DisplayName { get; }

        public EnumDisplayNameAttribute(String displayName)
        {
            this.DisplayName = displayName;
        }
    }
}
