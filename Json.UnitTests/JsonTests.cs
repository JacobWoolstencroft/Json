using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Json.UnitTests
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

        [TestMethod]
        public void ToJsonStringTest()
        {
            string jsonIn, jsonOut, correctIndent, correctNonIndent;
            JsonToken token;

            jsonIn = "{ \"int1\"   :   1700 ,  \"str1\"   : null, \"str2\":\"null\" ,\"dec1\":0.000000735, \n"+
                     "\"SubMap\"\t:{\"1\":1, \"emptyArray\":[],\n"+
                     "\t\"array\":[2,3, 7 , \"MOO\"]}}";
            correctIndent = "{\n"+
                      "\t\"int1\":1700,\n"+
                      "\t\"str1\":null,\n"+
                      "\t\"str2\":\"null\",\n"+
                      "\t\"dec1\":0.000000735,\n"+
                      "\t\"SubMap\":{\n"+
                      "\t\t\"1\":1,\n"+
                      "\t\t\"emptyArray\":[],\n"+
                      "\t\t\"array\":[\n"+
                      "\t\t\t2,\n"+
                      "\t\t\t3,\n"+
                      "\t\t\t7,\n"+
                      "\t\t\t\"MOO\"\n"+
                      "\t\t]\n"+
                      "\t}\n"+
                      "}";
            correctNonIndent = correctIndent.Replace("\n", "").Replace("\t", "");
            correctIndent = correctIndent.Replace("\n", Environment.NewLine);

            token = JsonToken.Parse(jsonIn);

            jsonOut = token.ToJsonString(true);
            Assert.AreEqual(correctIndent, jsonOut);

            jsonOut = token.ToJsonString(false);
            Assert.AreEqual(correctNonIndent, jsonOut);
        }
    }
}
