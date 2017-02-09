using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void Max_Equal()
        {
            var value1 = DateTimeOffset.MaxValue;
            var value2 = DateTimeOffset.MaxValue;

            Assert.IsTrue(UptimeMath.Max(value1, value2) == value1);
        }

        [TestMethod]
        public void Max_FirstGreaterThanSecond()
        {
            var value1 = DateTimeOffset.MaxValue;
            var value2 = DateTimeOffset.MinValue;

            Assert.IsTrue(UptimeMath.Max(value1, value2) == value1);
        }

        [TestMethod]
        public void Max_SecondGreaterThanFirst()
        {
            var value1 = DateTimeOffset.MinValue;
            var value2 = DateTimeOffset.MaxValue;

            Assert.IsTrue(UptimeMath.Max(value1, value2) == value2);
        }
    }
}
