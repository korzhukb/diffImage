using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffImage.Service;

namespace DiffImage.Tests.Service
{
    [TestClass]
    public class CompareImgTests
    {
        [TestMethod]
        public void CalculationMaxValue_1and50and40_40returned()
        {
            //arrage
            int point = 1;
            int length = 50;
            int maxLength = 40;
            int expected = 40;

            //act
            int actual = CompareImgService.CalculationMaxValue(point, length, maxLength);

            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
