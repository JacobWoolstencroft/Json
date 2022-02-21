using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Json
{
    [System.AttributeUsage(AttributeTargets.Field)]
    internal class FriendlyNameAttribute : System.Attribute
    {
        public readonly string FriendlyName;
        public bool Hidden;

        public FriendlyNameAttribute(string FriendlyName)
        {
            this.FriendlyName = FriendlyName;
        }
    }

    internal static class FriendlyNameHelpers
    {
        public static string FriendlyName(this Enum e)
        {
            MemberInfo[] members = e.GetType().GetMember(e.ToString());
            if (members != null && members.Length > 0)
            {
                FriendlyNameAttribute friendlyName = members[0].GetCustomAttribute<FriendlyNameAttribute>();
                if (friendlyName != null)
                    return friendlyName.FriendlyName;
            }
            return e.ToString();
        }
        public static bool IsHidden(this Enum e)
        {
            MemberInfo[] members = e.GetType().GetMember(e.ToString());
            if (members != null && members.Length > 0)
            {
                FriendlyNameAttribute friendlyName = members[0].GetCustomAttribute<FriendlyNameAttribute>();
                if (friendlyName != null)
                    return friendlyName.Hidden;
            }
            return false;
        }
    }
}
