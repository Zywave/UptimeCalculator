using System;
using System.Globalization;
using System.ComponentModel;

namespace UptimeCalculator
{
    public class IntervalConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                Interval interval;
                if (Interval.TryParse((string)value, out interval))
                {
                    return interval;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
