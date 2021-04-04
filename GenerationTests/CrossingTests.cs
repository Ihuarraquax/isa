using System.Reflection;
using GeneticAlgorithmModule.Models;
using NUnit.Framework;
namespace GenerationTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CrossToParent1()
        {
            var result = Generation.CrossToParents("11111111", "00000000", 2);
            Assert.AreEqual("11100000", result);
        }
        
        [Test]
        public void CrossToParent2()
        {
            var result = Generation.CrossToParents("11111111", "00000000", 0);
            Assert.AreEqual("10000000", result);
        }
        
        [Test]
        public void CrossToParent3()
        {
            var result = Generation.CrossToParents("11111111", "00000000", 6);
            Assert.AreEqual("11111110", result);
        }
    }
}