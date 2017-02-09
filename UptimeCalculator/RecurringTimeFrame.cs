using System;

namespace UptimeCalculator
{
    public struct RecurringTimeFrame
    {
        public RecurringTimeFrame(TimeSpan startTime, TimeSpan endTime, TimeZoneInfo timeZone, DaysOfWeek daysOfWeek)
        {
            StartTime = startTime;
            EndTime = endTime;
            TimeZone = timeZone;
            DaysOfWeek = daysOfWeek;
        }

        public TimeSpan StartTime { get; }

        public TimeSpan EndTime { get; }

        public TimeZoneInfo TimeZone { get; }

        public DaysOfWeek DaysOfWeek { get; }
    }
}
