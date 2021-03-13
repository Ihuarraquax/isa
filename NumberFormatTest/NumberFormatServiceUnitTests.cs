using System;
using System.Collections.Generic;
using NumberFormatManager.Services;
using NUnit.Framework;

namespace NumberFormatManagerTest
{
    public class Tests
    {
        private NumberFormatService _numberFormatService;

        [SetUp]
        public void Setup()
        {
            _numberFormatService = new NumberFormatService(-2, 3, 0.001m);
        }

        [Test]
        public void RealToInt()
        {
            var result = _numberFormatService.RealToInt(-1.234m);
            Assert.AreEqual(1255, result);
        }

        [Test]
        public void IntToBin()
        {
            var result = _numberFormatService.IntToBin(1225);
            Assert.AreEqual("0010011001001", result);
        }

        [Test]
        public void BinToInt()
        {
            var result = _numberFormatService.BinToInt("0010011001001");
            Assert.AreEqual(1225, result);
        }

        [Test]
        public void IntToReal()
        {
            var result = _numberFormatService.IntToReal(4095);
            Assert.AreEqual(0.5m, result);
        }
        
        [Test]
        public void BinToReal()
        {
            var result = _numberFormatService.BinToReal("10011001001");
            Assert.AreEqual(-1.252m, result);
        }
        
        [Test]
        public void RealToBin()
        {
            var result = _numberFormatService.RealToBin(-1.234m);
            Assert.AreEqual("0010011100111", result);
        }
        
        [Test]
        public void RandomDecimal()
        {
            decimal? invalidNumber = null;

            for (var i = 0; i < 10000; i++)
            {
                var randomDecimal = _numberFormatService.RandomDecimal();
                
                if (decimal.Compare(randomDecimal, _numberFormatService.A) < 0  || decimal.Compare(randomDecimal, _numberFormatService.B) > 0)
                {
                    invalidNumber = randomDecimal;
                    break;
                }
            }

            Assert.IsNull(invalidNumber);
        }
        
        [Test]
        public void L()
        {
            Assert.AreEqual(13, _numberFormatService.L);
        }
        
        [Test]
        public void TestAllTransformations()
        {
            List<decimal> invalidValues = new();
            for (int i = 0; i < 10000; i++)
            {
                var generatedXReal = _numberFormatService.RandomDecimal();
                var xIntFromGeneratedXReal = _numberFormatService.RealToInt(generatedXReal);
                var xBinFromXInt = _numberFormatService.IntToBin(xIntFromGeneratedXReal);
                var xIntFromXBin = _numberFormatService.BinToInt(xBinFromXInt);
                var xRealFromXInt = _numberFormatService.IntToReal(xIntFromXBin);

                if (generatedXReal != xRealFromXInt || xIntFromGeneratedXReal != xIntFromXBin)
                {
                    invalidValues.Add(generatedXReal);
                }
            }
            
            Assert.IsEmpty(invalidValues);
        }
    }
}