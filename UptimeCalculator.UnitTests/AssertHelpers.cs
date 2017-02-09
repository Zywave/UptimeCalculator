using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public static class AssertHelpers
    {
        public static void AreClose(double value1, double value2, double tolerance)
        {
            Assert.IsTrue(Math.Abs(value1 - value2) <= Math.Abs(value1 * tolerance));
        }
    }
}
