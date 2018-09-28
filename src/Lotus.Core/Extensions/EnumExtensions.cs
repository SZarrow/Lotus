using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Lotus.Core.Extensions
{
    public static class EnumExtensions
    {
        public static T GetValue<T>(this Enum obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            var field = obj.GetType().GetField(obj.ToString());
            var cusAttr = field.GetCustomAttribute<EnumValueAttribute>();
            if (cusAttr != null)
            {
                if (cusAttr.Value is T)
                {
                    return (T)cusAttr.Value;
                }

                if (typeof(T) == typeof(String))
                {
                    Object value = cusAttr.Value != null ? cusAttr.Value.ToString() : obj.ToString();
                    return (T)value;
                }
            }

            return default(T);
        }

        public static String GetValue(this Enum obj)
        {
            return GetValue<String>(obj);
        }
    }
}
