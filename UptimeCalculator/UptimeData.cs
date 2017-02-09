using System.Collections.Generic;

namespace UptimeCalculator
{
    public class UptimeData
    {
        public IEnumerable<Interval> Downtimes { get; set; }

        public IEnumerable<Interval> Exclusions { get; set; }

        public IEnumerable<RecurringTimeFrame> BusinessHours { get; set; }
    }
}
