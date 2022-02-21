using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json
{
    internal static class helpers
    {
        public static string SafeSubstring(this string str, int start, int length)
        {
            if (str == null || length <= 0)
                return "";
            if (start >= str.Length)
                return "";
            if (start + length > str.Length)
                return str.Substring(start);
            return str.Substring(start, length);
        }
        public static string SafeSubstring(this string str, int start)
        {
            if (str == null)
                return "";
            if (start >= str.Length)
                return "";
            return str.Substring(start);
        }
    }
}
