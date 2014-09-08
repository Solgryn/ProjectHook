using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProjectHook
{
    public static class ExtensionMethods
    {
        public static float SmoothTowards(this float from, float to, float coefficient)
        {
            return from + (to - from)*coefficient;
        }

        //Get description attribute of enums
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }
    }
}
