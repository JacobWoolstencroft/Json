using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses
{
    public abstract class AbstractTestClass : IJsonPackable
    {
        public string str;
        public abstract string GetText();
        public abstract JsonToken Pack();
        public abstract void Unpack(JsonToken json);

        public void AssertMatch(AbstractTestClass other, string Name)
        {
            Assert.AreNotSame(this, other, Name + " is the same object");
            Assert.AreEqual(str, other.str, Name + ".str does not match");
            AssertMatchSubclass(other, Name);
        }
        public abstract void AssertMatchSubclass(AbstractTestClass other, string Name);
    }
}
