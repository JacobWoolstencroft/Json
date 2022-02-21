using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Json;

namespace UnitTests
{
    [TestClass]
    public class JsonTests
    {
        [TestMethod]
        public void ArrayTest1()
        {
            string json;
            JsonToken token;
            int i;
            long l;
            decimal d;
            string s;

            json = "[ 17e2, 3.5, -409348274847, 3482851, \"MOO\" \t,\"37.5\"]";
            token = JsonToken.Parse(json);

            if (token is JsonArray array)
            {
                #region Value tests
                Assert.IsTrue(array[0].TryGetInt(out i) && i == 1700, "json[0] should be 1700");
                Assert.IsTrue(array[1].TryGetDecimal(out d) && d == 3.5M, "json[1] should be 3.5");
                Assert.IsTrue(array[2].TryGetLong(out l) && l == -409348274847, "json[2] should be -409348274847");
                Assert.IsTrue(array[3].TryGetInt(out i) && i == 3482851, "json[3] should be 3482851");
                Assert.IsTrue(array[4].TryGetString(out s) && s == "MOO", "json[4] should be \"MOO\"");
                Assert.IsTrue(array[5].TryGetString(out s) && s == "37.5", "json[5] should be 37.5");
                #endregion

                #region Value conversions
                Assert.IsTrue(array[0].TryGetLong(out l) && l == 1700, "1700 should convert to long 1700");
                Assert.IsFalse(array[1].TryGetInt(out i) || array[1].TryGetLong(out l), "3.5 should not convert to integer or long");
                Assert.IsFalse(array[2].TryGetInt(out i), "-409348274847 should not convert to int (out of range)");
                Assert.IsTrue(array[3].TryGetString(out s) && s == "3482851", "3482851 should convert to string \"3482851\"");
                Assert.IsFalse(array[4].TryGetDecimal(out d), "\"MOO\" should not convert to decimal");
                Assert.IsTrue(array[5].TryGetDecimal(out d) && d == 37.5M, "\"37.5\" should convert to decimal 37.5");
                #endregion

            }
            else
                Assert.Fail("json should parse as a JsonArray");
        }

        [TestMethod]
        public void AbstractArrayTest1()
        {
            List<AbstractTestClass> list = new List<AbstractTestClass>();
            ConcreteTestClass1 a = new ConcreteTestClass1();
            a.str = "A";
            a.str2 = "A2";
            ConcreteTestClass2 b = new ConcreteTestClass2();
            b.str = "B";
            b.i = 42;

            list.Add(a);
            list.Add(b);

            JsonPackager packager = new JsonPackager(Assembly.GetAssembly(typeof(AbstractTestClass)));
            string json = packager.Package(list).ToJsonString();

            List<AbstractTestClass> list2 = packager.UnpackageList<AbstractTestClass>(json);
            Assert.IsTrue(list2.Count == 2, "List did not parse as the correct number of elements");
            if (list2[0] is ConcreteTestClass1 a2)
            {
                Assert.IsTrue(a2.str == "A", "ConcreteTestClass1.str failed to parse as the correct value");
                Assert.IsTrue(a2.str2 == "A2", "ConcreteTestClass1.str2 failed to parse as the correct value");
            }
            else
                Assert.Fail("ConcreteTestClass1 failed to unpackage as ConcreteTestClass1");
            if (list2[1] is ConcreteTestClass2 b2)
            {
                Assert.IsTrue(b2.str == "B", "ConcreteTestClass2.str failed to parse as the correct value");
                Assert.IsTrue(b2.i == 42, "ConcreteTestClass2.i failed to parse as the correct value");
            }
            else
                Assert.Fail("ConcreteTestClass2 failed to unpackage as ConcreteTestClass1");
        }

        [TestMethod]
        public void MappingTest1()
        {
            string json;
            JsonToken token;
            int i;
            long l;
            decimal d;
            string s;

            json = "{ \"int1\"   :   17e2 ,  \"str1\"   : null, \"str2\":\"null\" ,\"dec1\":7.35e-7}";
            token = JsonToken.Parse(json);

            if (token is JsonMapping map)
            {
                #region Value tests
                Assert.IsTrue(map["int1"].TryGetInt(out i) && i == 1700, "map[\"int1\"] should be 1700");
                Assert.IsTrue(map["str1"].TryGetString(out s) && s == null, "map[\"str1\"] should be null");
                Assert.IsTrue(map["str2"].TryGetString(out s) && s == "null", "map[\"str2\"] should be string \"null\"");
                Assert.IsTrue(map["dec1"].TryGetDecimal(out d) && d == 7.35e-7M, "map[\"dec1\"] should be 7.35e-7");
                Assert.ThrowsException<System.Collections.Generic.KeyNotFoundException>(() => map["moo"], "map[\"moo\"] should not be found, and should throw an exception");
                #endregion
            }
            else
                Assert.Fail("json should parse as a JsonMapping");
        }
    }

    public abstract class AbstractTestClass : IJsonPackable
    {
        public string str;
        public abstract string GetText();
        public abstract JsonToken Pack();
        public abstract void Unpack(JsonToken json);
    }
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
    }
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
    }
}
