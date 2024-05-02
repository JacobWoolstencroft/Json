using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses
{
    public class ConcreteTestClass2 : AbstractTestClass
    {
        public int i;
        public override string GetText()
        {
            return str + " i=" + i;
        }

        public override JsonToken Pack()
        {
            JsonMapping map = new JsonMapping();
            map["str"] = str;
            map["i"] = i;
            return map;
        }

        public override void Unpack(JsonToken json)
        {
            str = json.GetString("str", null);
            i = json.GetInt("i", 0);
        }

        public override void AssertMatchSubclass(AbstractTestClass other, string Name)
        {
            if (other is ConcreteTestClass2 other2)
            {
                Assert.AreEqual(i, other2.i, Name + ".i does not match");
            }
            else
                Assert.Fail(Name + " has a type mismatch");
        }
    }
}
