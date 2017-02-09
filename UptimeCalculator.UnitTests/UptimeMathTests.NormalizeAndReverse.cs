using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void NormalizeAndReverse_NoIntervals()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var intervals = new List<Interval>();

            var normalizedIntervals = UptimeMath.NormalizeAndReverse(intervals, period).ToList();

            Assert.AreEqual(1, normalizedIntervals.Count());
            Assert.AreEqual(period, normalizedIntervals[0]);
        }

        [TestMethod]
        public void NormalizeAndReverse_SingleInterval()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var intervals = new List<Interval>()
            {
                new Interval(now.AddDays(-3), now.AddDays(-2))
            };

            var normalizedIntervals = UptimeMath.NormalizeAndReverse(intervals, period).ToList();
            
            Assert.AreEqual(2, normalizedIntervals.Count());
            Assert.AreEqual(new Interval(now.AddDays(-30), now.AddDays(-3)), normalizedIntervals[0]);
            Assert.AreEqual(new Interval(now.AddDays(-2), now), normalizedIntervals[1]);
        }

        [TestMethod]
        public void NormalizeAndReverse_MultipleIntervalsNoOverlap()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var intervals = new List<Interval>()
            {
                new Interval(now.AddDays(-10), now.AddDays(-9)),
                new Interval(now.AddDays(-20), now.AddDays(-19))
            };

            var normalizedIntervals = UptimeMath.NormalizeAndReverse(intervals, period).ToList();

            Assert.AreEqual(3, normalizedIntervals.Count());
            Assert.AreEqual(new Interval(now.AddDays(-30), now.AddDays(-20)), normalizedIntervals[0]);
            Assert.AreEqual(new Interval(now.AddDays(-19), now.AddDays(-10)), normalizedIntervals[1]);
            Assert.AreEqual(new Interval(now.AddDays(-9), now), normalizedIntervals[2]);
        }

        [TestMethod]
        public void NormalizeAndReverse_MultipleIntervalsOverlap()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var intervals = new List<Interval>()
            {
                new Interval(now.AddDays(-10), now.AddDays(-9)),
                new Interval(now.AddDays(-9.5), now.AddDays(-5)),
                new Interval(now.AddDays(-6), now.AddDays(-2))
            };

            var normalizedIntervals = UptimeMath.NormalizeAndReverse(intervals, period).ToList();

            Assert.AreEqual(2, normalizedIntervals.Count());
            Assert.AreEqual(new Interval(now.AddDays(-30), now.AddDays(-10)), normalizedIntervals[0]);
            Assert.AreEqual(new Interval(now.AddDays(-2), now), normalizedIntervals[1]);
        }
    }
}
