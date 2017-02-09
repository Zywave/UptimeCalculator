using System;
using System.Collections.Generic;
using System.Linq;

namespace UptimeCalculator
{
    public struct Uptime
    {
        public Uptime(TimeSpan totalTime, IEnumerable<Interval> downtimes)
        {
            TotalTime = totalTime;
            TotalUptime = totalTime;
            TotalDowntime = TimeSpan.Zero;
            PercentUptime = 1.0;
            PercentDowntime = 0.0;
            ShortestDowntime = TimeSpan.Zero;
            LongestDowntime = TimeSpan.Zero;
            AverageDowntime = TimeSpan.Zero;

            downtimes = downtimes as List<Interval> ?? downtimes?.ToList() ?? new List<Interval>();
            var downtimeSpans = downtimes.Select(d => d.TimeSpan).ToList();

            DowntimeCount = downtimeSpans.Count;

            if (DowntimeCount > 0)
            {
                var orderedDowntimeSpans = downtimeSpans.OrderBy(ts => ts);

                var downtimeSeconds = downtimeSpans.Sum(t => t.TotalSeconds);

                var percentDowntime = downtimeSeconds / totalTime.TotalSeconds;

                TotalUptime = totalTime - TimeSpan.FromSeconds(downtimeSeconds);
                TotalDowntime = TimeSpan.FromSeconds(downtimeSeconds);
                PercentUptime = 1 - percentDowntime;
                PercentDowntime = percentDowntime;
                DowntimeCount = DowntimeCount;
                AverageDowntime = TimeSpan.FromSeconds(downtimeSeconds / DowntimeCount);
                ShortestDowntime = orderedDowntimeSpans.First();
                LongestDowntime = orderedDowntimeSpans.Last();
            }
        }

        public TimeSpan TotalTime { get; }

        public TimeSpan TotalUptime { get; }

        public TimeSpan TotalDowntime { get; }

        public double PercentUptime { get; }

        public double PercentDowntime { get; }

        public int DowntimeCount { get; }

        public TimeSpan ShortestDowntime { get; }

        public TimeSpan LongestDowntime { get; }

        public TimeSpan AverageDowntime { get; }

        public static Uptime Calculate(Interval period, UptimeData data)
        {
            return UptimeMath.Calculate(period, data);
        }
    }
}
