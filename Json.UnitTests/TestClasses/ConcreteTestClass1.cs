using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses
{
    public class ConcreteTestClass1 : AbstractTestClass
    {
        public string str2;
        public override string GetText()
        {
            return str + " " + str2;
        }

        public override JsonToken Pack()
        {
            JsonMapping map = new JsonMapping();
            map["str"] = str;
            map["str2"] = str2;
            return map;
        }

        public override void Unpack(JsonToken json)
        {
            str = json.GetString("str", null);
            str2 = json.GetString("str2", null);
        }

        public override void AssertMatchSubclass(AbstractTestClass other, string Name)
        {
            if (other is ConcreteTestClass1 other1)
            {
                Assert.AreEqual(str2, other1.str2, Name + ".str2 does not match");
            }
            else
                Assert.Fail(Name + " has a type mismatch");
        }
    }
}
