﻿using System;
using System.Collections.Generic;
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
            if (cusAttr != null && cusAttr.Value is T)
            {
                return (T)cusAttr.Value;
            }

            return default(T);
            throw new NotImplementedException();
        }
    }
}
