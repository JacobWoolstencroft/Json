using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using System.Collections.Generic;
using Json.UnitTests.TestClasses;
using Json.UnitTests.TestClasses.NonJsonPackable;

namespace Json.UnitTests
{
    [TestClass]
    public class PackagerTests
    {
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

            List<AbstractTestClass> list2 = packager.Unpackage<List<AbstractTestClass>>(json);
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
        public void NonJsonPackableTest()
        {
            List<Animal> list = new List<Animal>();
            Dog dog;
            Cat cat;
            string json = null;

            dog = new Dog();
            dog.Name = "Fido";
            dog.Breed = "Black Lab";
            dog.IsGoodBoy = true;
            list.Add(dog);

            cat = new Cat();
            cat.Name = "Basement Cat";
            cat.EvilQuotient = 10;
            list.Add(cat);

            cat = new Cat();
            cat.Name = "Ceiling Cat";
            cat.EvilQuotient = -7;
            list.Add(cat);

            JsonPackager packager = new JsonPackager(Assembly.GetAssembly(typeof(Animal)));

            Assert.ThrowsException<JsonPackager.JsonPackagerException>(() =>
                json = packager.Package(list).ToJsonString()
            );


            //List<Animal> list2 = packager.Unpackage<List<Animal>>(json);
            //Assert.IsTrue(list2.Count == 3, "List did not parse as the correct number of elements");

            //for (int x = 0; x < list.Count; x++)
            //    list[x].AssertMatch(list2[x]);

        }

        [TestMethod]
        public void NestedArraysTest()
        {
            List<List<AbstractTestClass>> mainList1 = new List<List<AbstractTestClass>>();

            for (int x = 0; x < 4; x++)
            {
                List<AbstractTestClass> list = new List<AbstractTestClass>();
                for (int y = 0; y < x; y++)
                {
                    ConcreteTestClass1 a = new ConcreteTestClass1();
                    a.str = "Class1 x=" + x + " y=" + y;
                    a.str2 = "(" + x + ", " + y + ")";

                    ConcreteTestClass2 b = new ConcreteTestClass2();
                    b.str = "Class2 x=" + x + " y=" + y;
                    b.i = x + y;

                    list.Add(a);
                    list.Add(b);
                }
                mainList1.Add(list);
            }


            JsonPackager packager = new JsonPackager(Assembly.GetAssembly(typeof(AbstractTestClass)));
            string json = packager.Package(mainList1).ToJsonString();

            List<List<AbstractTestClass>> mainList2 = packager.Unpackage<List<List<AbstractTestClass>>>(json);
            Assert.IsTrue(mainList1.Count == mainList2.Count, "List did not parse as the correct number of elements");
            for (int x = 0; x < mainList1.Count; x++)
            {
                Assert.IsTrue(mainList1[x].Count == mainList2[x].Count, "List[" + x + "] did not parse as the correct number of elements");
                for (int y = 0; y < mainList1[x].Count; y++)
                {
                    AbstractTestClass ob1, ob2;
                    ob1 = mainList1[x][y];
                    ob2 = mainList2[x][y];

                    ob1.AssertMatch(ob2, "List[" + x + "][" + y + "]");
                }
            }
        }
    }
}
