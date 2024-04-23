using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Json.UnitTests.TestClasses.NonJsonPackable
{
    public abstract class Animal
    {
        public string Name;

        public void AssertMatch(Animal otherAnimal)
        {
            Assert.AreNotSame(this, otherAnimal);
            Assert.AreEqual(Name, otherAnimal.Name);
        }
        public abstract void AssertMatchSpecies(Animal otherAnimal);
    }
}
