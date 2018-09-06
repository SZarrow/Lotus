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

        private String _displayName;

        public String DisplayName
        {
            get { return _displayName; }
        }

        public EnumDisplayNameAttribute(String displayName)
        {
            _displayName = displayName;
        }
    }
}
