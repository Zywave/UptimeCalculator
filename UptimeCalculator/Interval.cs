using System;

namespace UptimeCalculator
{
    public struct Interval : IEquatable<Interval>, IFormattable
    {
        public Interval(DateTimeOffset start, DateTimeOffset end)
        {
            if (end < start) throw new ArgumentOutOfRangeException(nameof(end));

            Start = start;
            End = end;
            TimeSpan = end - start;
        }

        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public TimeSpan TimeSpan { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Interval))
            {
                return false;
            }

            return Equals((Interval) obj);
        }

        public bool Equals(Interval other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override string ToString()
        {
            return $"{Start} -> {End}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{Start.ToString(format, formatProvider)} -> {End.ToString(format, formatProvider)}";
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }
    }
}
