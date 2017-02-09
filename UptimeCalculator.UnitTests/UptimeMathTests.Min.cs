using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void Min_Equal()
        {
            var value1 = DateTimeOffset.MaxValue;
            var value2 = DateTimeOffset.MaxValue;

            Assert.IsTrue(UptimeMath.Min(value1, value2) == value1);
        }

        [TestMethod]
        public void Min_FirstGreaterThanSecond()
        {
            var value1 = DateTimeOffset.MaxValue;
            var value2 = DateTimeOffset.MinValue;

            Assert.IsTrue(UptimeMath.Min(value1, value2) == value2);
        }

        [TestMethod]
        public void Min_SecondGreaterThanFirst()
        {
            var value1 = DateTimeOffset.MinValue;
            var value2 = DateTimeOffset.MaxValue;

            Assert.IsTrue(UptimeMath.Min(value1, value2) == value1);
        }
    }
}
