using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void Normalize_NoIntervals()
        {
            var intervals = new List<Interval>();

            var normalizedIntervals = UptimeMath.Normalize(intervals);

            Assert.IsTrue(!normalizedIntervals.Any());
        }

        [TestMethod]
        public void Normalize_SingleInterval()
        {
            var intervals = new List<Interval>()
            {
                new Interval(DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
            };

            var normalizedIntervals = UptimeMath.Normalize(intervals).ToList();
            
            Assert.AreEqual(1, normalizedIntervals.Count());
            Assert.IsTrue(!intervals.Except(normalizedIntervals).Any());
        }

        [TestMethod]
        public void Normalize_MultipleIntervalsNoOverlap()
        {
            var now = DateTimeOffset.Now;

            var intervals = new List<Interval>()
            {
                new Interval(now.AddDays(-10), now.AddDays(-9)),
                new Interval(now.AddDays(-20), now.AddDays(-19))
            };

            var normalizedIntervals = UptimeMath.Normalize(intervals).ToList();
            
            Assert.AreEqual(2, normalizedIntervals.Count());
            Assert.IsTrue(!intervals.Except(normalizedIntervals).Any());
        }

        [TestMethod]
        public void Normalize_MultipleIntervalsOverlap()
        {
            var now = DateTimeOffset.Now;

            var intervals = new List<Interval>()
            {
                new Interval(now.AddDays(-10), now.AddDays(-9)),
                new Interval(now.AddDays(-9.5), now.AddDays(-5)),
                new Interval(now.AddDays(-6), now.AddDays(-2))
            };

            var normalizedIntervals = UptimeMath.Normalize(intervals).ToList();

            Assert.AreEqual(1, normalizedIntervals.Count());
            Assert.AreEqual(new Interval(now.AddDays(-10), now.AddDays(-2)), normalizedIntervals[0]);
        }
    }
}
