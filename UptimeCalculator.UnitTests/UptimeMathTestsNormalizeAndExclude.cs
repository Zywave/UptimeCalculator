using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void NormalizeAndExclude_NoIntervals()
        {
            var now = DateTimeOffset.Now;
            
            var intervals = new List<Interval>();
            var exclusions = new List<Interval>();

            var result = UptimeMath.NormalizeAndExclude(intervals, exclusions);

            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void NormalizeAndExclude_CompleteInterval()
        {
            var now = DateTimeOffset.Now;
            
            var intervals = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51))
            };
            var exclusions = new List<Interval>();

            var result = UptimeMath.NormalizeAndExclude(intervals, exclusions).ToList();

            Assert.IsTrue(!intervals.Except(result).Any());
        }

        [TestMethod]
        public void NormalizeAndExclude_MultipleOutagesCompleteExclusion()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var intervals = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51)),
                new Interval(now.AddHours(-20), now.AddHours(-18)),
                new Interval(now.AddHours(-53), now.AddHours(-51.5))
            };
            var exclusions = new List<Interval>()
            {
                period
            };

            var result = UptimeMath.NormalizeAndExclude(intervals, exclusions).ToList();

            Assert.IsTrue(!result.Any());
        }

        [TestMethod]
        public void NormalizeAndExclude_SingleOutageSingleExclusionOverlap()
        {
            var now = DateTimeOffset.Now;

            var intervals = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51))
            };
            var exclusions = new List<Interval>()
            {
                new Interval(now.AddHours(-52.5), now.AddHours(-51.5))
            };

            var result = UptimeMath.NormalizeAndExclude(intervals, exclusions).ToList();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(new Interval(now.AddHours(-51.5), now.AddHours(-51)), result[0]);
        }
    }
}
