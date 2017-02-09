using System;
using System.Collections.Generic;
using System.Linq;

namespace UptimeCalculator
{
    internal static class UptimeMath
    {
        internal static Uptime Calculate(Interval period, UptimeData data)
        {
            var downtimes = data?.Downtimes?.ToList() ?? new List<Interval>();
            var exclusions = data?.Exclusions?.ToList() ?? new List<Interval>();

            downtimes = Slice(downtimes, period).ToList();
            exclusions = Slice(exclusions, period).ToList();

            var nonBusinessHours = new List<Interval>();
            if (data?.BusinessHours != null)
            {
                var businessHoursTimeFrames = data.BusinessHours?.ToList() ?? new List<RecurringTimeFrame>();
                var businessHours = GetRecurringTimeFrameIntervals(businessHoursTimeFrames, period).ToList();
                nonBusinessHours = NormalizeAndReverse(businessHours, period).ToList();
            }

            exclusions.AddRange(nonBusinessHours);

            downtimes = NormalizeAndExclude(downtimes, exclusions).ToList();
           
            var totalTime = period.TimeSpan - new TimeSpan(nonBusinessHours.Sum(ts => ts.TimeSpan.Ticks));

            return new Uptime(totalTime, downtimes);
        }

        internal static IEnumerable<Interval> GetRecurringTimeFrameIntervals(IEnumerable<RecurringTimeFrame> timeFrames, Interval limit)
        {
            timeFrames = timeFrames as List<RecurringTimeFrame> ?? timeFrames?.ToList() ?? new List<RecurringTimeFrame>();

            var current = limit.Start;
            do
            {
                var date = current.UtcDateTime.Date;
                foreach (var timeFrame in timeFrames)
                {
                    var utcOffset = timeFrame.TimeZone.GetUtcOffset(date);
                    if (timeFrame.DaysOfWeek.HasDayOfWeek(date.DayOfWeek))
                    {
                        yield return new Interval(
                            date + (timeFrame.StartTime - utcOffset),
                            date + (timeFrame.EndTime - utcOffset));
                    }
                }
            } while ((current = current.AddDays(1)) <= limit.End);
        }

        internal static IEnumerable<Interval> NormalizeAndExclude(IEnumerable<Interval> intervals, IEnumerable<Interval> exclusions)
        {
            intervals = Normalize(intervals).ToList();

            if (!intervals.Any())
            {
                yield break;
            }

            exclusions = Normalize(exclusions).ToList();

            if (!exclusions.Any())
            {
                foreach (var interval in intervals)
                {
                    yield return interval;
                }
                yield break;
            }

            using (var intervalsEnumerator = intervals.GetEnumerator())
            using (var exclusionEnumerator = exclusions.GetEnumerator())
            {
                intervalsEnumerator.MoveNext();
                exclusionEnumerator.MoveNext();

                var interval = intervalsEnumerator.Current;
                var exclusion = exclusionEnumerator.Current;

                while (true)
                {
                    if (interval.Start <= exclusion.Start)
                    {
                        //interval is first
                        if (interval.End <= exclusion.Start)
                        {
                            // no overlap;
                            yield return interval;
                            if (intervalsEnumerator.MoveNext())
                            {
                                interval = intervalsEnumerator.Current;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if (interval.End > exclusion.End)
                        {
                            // complete overlap
                            yield return new Interval(interval.Start, exclusion.Start);
                            interval = new Interval(exclusion.End, interval.End);
                            if (exclusionEnumerator.MoveNext())
                            {
                                exclusion = exclusionEnumerator.Current;
                            }
                            else
                            {
                                yield return interval;
                                break;
                            }
                        }
                        else
                        {
                            // partial overlap
                            yield return new Interval(interval.Start, exclusion.Start);
                            if (intervalsEnumerator.MoveNext())
                            {
                                interval = intervalsEnumerator.Current;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        //exclusion is first
                        if (interval.Start >= exclusion.End)
                        {
                            // no overlap
                            if (exclusionEnumerator.MoveNext())
                            {
                                exclusion = exclusionEnumerator.Current;
                            }
                            else
                            {
                                yield return interval;
                                break;
                            }
                        }
                        else if (interval.End <= exclusion.End)
                        {
                            // complete overlap
                            if (intervalsEnumerator.MoveNext())
                            {
                                interval = intervalsEnumerator.Current;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            // partial overlap
                            interval = new Interval(exclusion.End, interval.End);
                            if (exclusionEnumerator.MoveNext())
                            {
                                exclusion = exclusionEnumerator.Current;
                            }
                            else
                            {
                                yield return interval;
                                break;
                            }
                        }
                    }
                }

                while (intervalsEnumerator.MoveNext())
                {
                    yield return intervalsEnumerator.Current;
                }
            }
        }

        internal static IEnumerable<Interval> NormalizeAndReverse(IEnumerable<Interval> intervals, Interval limit)
        {
            intervals = Normalize(intervals).SkipWhile(i => i.End < limit.Start).TakeWhile(i => i.Start < limit.End).ToList();
            
            if (!intervals.Any())
            {
                yield return limit;
                yield break;
            }

            var from = limit.Start;
            if (intervals.First().Start <= from)
            {
                from = intervals.First().End;
                intervals = intervals.Skip(1);
            }
            foreach (var interval in intervals)
            {
                yield return new Interval(from, interval.Start);
                from = interval.End;
            }
            if (from < limit.End)
            {
                yield return new Interval(from, limit.End);
            }
        }

        internal static IEnumerable<Interval> Normalize(IEnumerable<Interval> intervals)
        {
            intervals = intervals as List<Interval> ?? intervals?.ToList() ?? new List<Interval>();

            if (!intervals.Any())
            {
                yield break;
            }

            intervals = intervals.OrderBy(i => i.Start);

            var accumulator = intervals.First();
            intervals = intervals.Skip(1);

            foreach (var interval in intervals)
            {
                if (interval.Start <= accumulator.End)
                {
                    accumulator = new Interval(accumulator.Start, Max(accumulator.End, interval.End));
                }
                else
                {
                    yield return accumulator;
                    accumulator = interval;
                }
            }

            yield return accumulator;
        }

        internal static IEnumerable<Interval> Slice(IEnumerable<Interval> intervals, Interval limit)
        {
            intervals = intervals as List<Interval> ?? intervals?.ToList() ?? new List<Interval>();

            return intervals.Where(i => i.End > limit.Start && i.Start < limit.End).Select(i => new Interval(Max(i.Start, limit.Start), Min(i.End, limit.End)));
        }

        internal static DateTimeOffset Min(DateTimeOffset dateTime1, DateTimeOffset dateTime2)
        {
            return dateTime1 < dateTime2 ? dateTime1 : dateTime2;
        }

        internal static DateTimeOffset Max(DateTimeOffset dateTime1, DateTimeOffset dateTime2)
        {
            return dateTime1 > dateTime2 ? dateTime1 : dateTime2;
        }
    }
}
