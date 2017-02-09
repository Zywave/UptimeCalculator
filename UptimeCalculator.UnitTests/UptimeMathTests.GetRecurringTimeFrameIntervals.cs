using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class UptimeMathTests
    {
        [TestMethod]
        public void GetRecurringTimeFrameIntervals_SingleTimeFrameLocal()
        {
            var date = DateTimeOffset.Now.Date;

            var period = new Interval(date.AddDays(-1), date);

            var businessHours = new List<RecurringTimeFrame>()
            {
                new RecurringTimeFrame(new TimeSpan(6, 0, 0), new TimeSpan(20, 0, 0), TimeZoneInfo.Local, DaysOfWeek.Weekdays)
            };

            var intervals = UptimeMath.GetRecurringTimeFrameIntervals(businessHours, period).ToList();
			
			Assert.AreEqual(2, intervals.Count());
			Assert.AreEqual(new Interval(date.AddDays(-1).AddHours(6), date.AddDays(-1).AddHours(20)), intervals[0]);
            Assert.AreEqual(new Interval(date.AddHours(6), date.AddHours(20)), intervals[1]);
        }

        [TestMethod]
        public void GetRecurringTimeFrameIntervals_SingleTimeFrameUtc()
        {
            var date = DateTimeOffset.Now.Date;

            var period = new Interval(date.AddDays(-1), date);

            var businessHours = new List<RecurringTimeFrame>()
            {
                new RecurringTimeFrame(new TimeSpan(6, 0, 0), new TimeSpan(20, 0, 0), TimeZoneInfo.Utc, DaysOfWeek.Weekdays)
            };

            var intervals = UptimeMath.GetRecurringTimeFrameIntervals(businessHours, period).ToList();

            Assert.AreEqual(2, intervals.Count());
            Assert.AreEqual(new Interval(new DateTimeOffset(date.AddDays(-1).AddHours(6), TimeSpan.Zero), new DateTimeOffset(date.AddDays(-1).AddHours(20), TimeSpan.Zero)), intervals[0]);
            Assert.AreEqual(new Interval(new DateTimeOffset(date.AddHours(6), TimeSpan.Zero), new DateTimeOffset(date.AddHours(20), TimeSpan.Zero)), intervals[1]);
        }
    }
}
