using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Util
{
    /// <summary>
    /// Conversores para tipos DateOnly e TimeOnly para uso com Entity Framework
    /// </summary>
    public static class EntityFrameworkConverters
    {

        public static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = 
            new ValueConverter<DateOnly, DateTime>(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        public static readonly ValueConverter<DateOnly?, DateTime?> NullableDateOnlyConverter = 
            new ValueConverter<DateOnly?, DateTime?>(
                v => v == null ? null : v.Value.ToDateTime(TimeOnly.MinValue),
                v => v == null ? null : DateOnly.FromDateTime(v.Value));

        public static readonly ValueConverter<TimeOnly, TimeSpan> TimeOnlyConverter = 
            new ValueConverter<TimeOnly, TimeSpan>(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

        public static readonly ValueConverter<TimeOnly?, TimeSpan?> NullableTimeOnlyConverter = 
            new ValueConverter<TimeOnly?, TimeSpan?>(
                v => v == null ? null : v.Value.ToTimeSpan(),
                v => v == null ? null : TimeOnly.FromTimeSpan(v.Value));
    }


    public class DateOnlyToDateTimeConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyToDateTimeConverter() 
            : base(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
        {
        }
    }


    public class NullableDateOnlyToDateTimeConverter : ValueConverter<DateOnly?, DateTime?>
    {
        public NullableDateOnlyToDateTimeConverter() 
            : base(
                v => v == null ? null : v.Value.ToDateTime(TimeOnly.MinValue),
                v => v == null ? null : DateOnly.FromDateTime(v.Value))
        {
        }
    }
}