using System;

namespace UptimeCalculator
{
    [Flags]
    public enum DaysOfWeek
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekends = Saturday | Sunday,
        Everyday = Weekdays | Weekends
    }

    internal static class DaysOfWeekExtensions
    {
        public static bool HasDayOfWeek(this DaysOfWeek daysOfWeek, DayOfWeek dayOfWeek)
        {
            return daysOfWeek.HasFlag(FromDayOfWeek(dayOfWeek));
        }

        private static DaysOfWeek FromDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return DaysOfWeek.Sunday;
                case DayOfWeek.Monday: return DaysOfWeek.Monday;
                case DayOfWeek.Tuesday: return DaysOfWeek.Tuesday;
                case DayOfWeek.Wednesday: return DaysOfWeek.Wednesday;
                case DayOfWeek.Thursday: return DaysOfWeek.Thursday;
                case DayOfWeek.Friday: return DaysOfWeek.Friday;
                case DayOfWeek.Saturday: return DaysOfWeek.Saturday;
                default: return DaysOfWeek.Sunday;
            }
        }
    }
}
