using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    public partial class UptimeMathTests
    {
        [TestMethod]
        public void Calculate_NoDowntime()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();

            var uptime = UptimeMath.Calculate(period, data);
            
            Assert.AreEqual(1.0, uptime.PercentUptime);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(0.0, uptime.PercentDowntime);
            Assert.AreEqual(TimeSpan.Zero, uptime.TotalDowntime);
            Assert.AreEqual(0, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.Zero, uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.Zero, uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.Zero, uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_CompleteDowntime()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(period.Start, period.End)
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            Assert.AreEqual(0.0, uptime.PercentUptime);
            Assert.AreEqual(1.0, uptime.PercentDowntime);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(period.TimeSpan, uptime.TotalDowntime);
            Assert.AreEqual(1, uptime.DowntimeCount);
            Assert.AreEqual(period.TimeSpan, uptime.LongestDowntime);
            Assert.AreEqual(period.TimeSpan, uptime.ShortestDowntime);
            Assert.AreEqual(period.TimeSpan, uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_MultipleDowntimesCompleteExclusion()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51)),
                new Interval(now.AddHours(-20), now.AddHours(-18)),
                new Interval(now.AddHours(-53), now.AddHours(-51.5))
            };
            data.Exclusions = new List<Interval>()
            {
                period
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            Assert.AreEqual(1.0, uptime.PercentUptime);
            Assert.AreEqual(0.0, uptime.PercentDowntime);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.Zero, uptime.TotalDowntime);
            Assert.AreEqual(0, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.Zero, uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.Zero, uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.Zero, uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_SingleDowntime()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(1).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(1).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.TotalDowntime);
            Assert.AreEqual(1, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_MultipleDowntimes()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51)),
                new Interval(now.AddHours(-20), now.AddHours(-18)),
                new Interval(now.AddHours(-53), now.AddHours(-51.5)),
                new Interval(now.AddDays(-60), now.AddDays(-59)),
                new Interval(now.AddDays(59), now.AddDays(60))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(4).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(4).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(4), uptime.TotalDowntime);
            Assert.AreEqual(2, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_MultipleDowntimesSingleDowntimeOutOfBounds()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51)),
                new Interval(now.AddHours(-20), now.AddHours(-18)),
                new Interval(now.AddHours(-53), now.AddHours(-51.5)),
                new Interval(now.AddDays(-32), now.AddDays(-31))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(4).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(4).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(4), uptime.TotalDowntime);
            Assert.AreEqual(2, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_SingleDowntimeSingleExclusionNoOverlap()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51))
            };
            data.Exclusions = new List<Interval>()
            {
                new Interval(now.AddHours(-62), now.AddHours(-61))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(1).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(1).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.TotalDowntime);
            Assert.AreEqual(1, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_SingleDowntimeSingleExclusionOverlap()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51))
            };
            data.Exclusions = new List<Interval>()
            {
                new Interval(now.AddHours(-52.5), now.AddHours(-51.5))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(.5).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(.5).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(.5), uptime.TotalDowntime);
            Assert.AreEqual(1, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(.5), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(.5), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(.5), uptime.AverageDowntime);
        }

        [TestMethod]
        public void Calculate_MultipleDowntimesSingleExclusionOverlap()
        {
            var now = DateTimeOffset.Now;

            var period = new Interval(now.AddDays(-30), now);

            var data = new UptimeData();
            data.Downtimes = new List<Interval>()
            {
                new Interval(now.AddHours(-52), now.AddHours(-51)),
                new Interval(now.AddHours(-54), now.AddHours(-51.5))
            };
            data.Exclusions = new List<Interval>()
            {
                new Interval(now.AddHours(-52.5), now.AddHours(-51.5))
            };

            var uptime = UptimeMath.Calculate(period, data);
            
            AssertHelpers.AreClose((period.TimeSpan.TotalSeconds - TimeSpan.FromHours(2).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentUptime, 0.00001);
            AssertHelpers.AreClose(1 - (period.TimeSpan.TotalSeconds - TimeSpan.FromHours(2).TotalSeconds) / period.TimeSpan.TotalSeconds, uptime.PercentDowntime, 0.00001);
            Assert.AreEqual(period.TimeSpan, uptime.TotalTime);
            Assert.AreEqual(TimeSpan.FromHours(2), uptime.TotalDowntime);
            Assert.AreEqual(2, uptime.DowntimeCount);
            Assert.AreEqual(TimeSpan.FromHours(1.5), uptime.LongestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(.5), uptime.ShortestDowntime);
            Assert.AreEqual(TimeSpan.FromHours(1), uptime.AverageDowntime);
        }
    }
}
