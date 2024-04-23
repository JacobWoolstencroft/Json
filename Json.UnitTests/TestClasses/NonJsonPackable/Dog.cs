using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses.NonJsonPackable
{
    public class Dog : Animal
    {
        public string Breed;
        public bool IsGoodBoy;

        public override void AssertMatchSpecies(Animal otherAnimal)
        {
            if (otherAnimal is Dog otherDog)
            {
                Assert.AreEqual(Breed, otherDog.Breed);
                Assert.AreEqual(IsGoodBoy, otherDog.IsGoodBoy);
            }
            else
                Assert.Fail("Type mismatch");
        }
    }
}
