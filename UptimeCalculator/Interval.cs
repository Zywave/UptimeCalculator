using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace UptimeCalculator
{
    [TypeConverter(typeof(IntervalConverter))]
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
            return $"{Start},{End}";
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"{Start.ToString(format, formatProvider)},{End.ToString(format, formatProvider)}";
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public static Interval Parse(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            Interval interval;
            if (TryParse(input, out interval))
            {
                return interval;
            }

            throw new FormatException("The input does not represent a valid interval.");
        }

        public static bool TryParse(string input, out Interval interval)
        {
            interval = default(Interval);

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var parts = input.Split(',');
            if (parts.Length != 2)
            {
                return false;
            }

            DateTimeOffset start, end;

            if (!DateTimeOffset.TryParse(parts[0], out start))
            {
                return false;
            }

            if (!DateTimeOffset.TryParse(parts[1], out end))
            {
                return false;
            }

            if (end < start)
            {
                return false;
            }

            interval = new Interval(start, end);
            return true;
        }
    }
}
