using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses.NonJsonPackable
{
    public class Cat : Animal
    {
        public int EvilQuotient = 0;

        public override void AssertMatchSpecies(Animal otherAnimal)
        {
            if (otherAnimal is Cat otherCat)
            {
                Assert.AreEqual(EvilQuotient, otherCat.EvilQuotient);
            }
            else
                Assert.Fail("Type mismatch");
        }
    }
}
