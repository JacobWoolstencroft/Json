using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Json
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JsonPackageAttribute : Attribute
    {
        public string Name;
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonAttribute : Attribute
    {
        public string Name = null;
        public bool Ignore = false;
    }
}
