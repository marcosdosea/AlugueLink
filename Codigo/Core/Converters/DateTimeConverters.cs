using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Converters
{
    /// <summary>
    /// Conversores para tipos DateOnly e TimeOnly
    /// </summary>
    public static class DateTimeConverters
    {
        /// <summary>
        /// Conversor para DateOnly
        /// </summary>
        public static readonly ValueConverter<DateOnly, DateTime> DateOnlyConverter = 
            new ValueConverter<DateOnly, DateTime>(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v));

        /// <summary>
        /// Conversor para DateOnly nullable
        /// </summary>
        public static readonly ValueConverter<DateOnly?, DateTime?> NullableDateOnlyConverter = 
            new ValueConverter<DateOnly?, DateTime?>(
                v => v == null ? null : v.Value.ToDateTime(TimeOnly.MinValue),
                v => v == null ? null : DateOnly.FromDateTime(v.Value));

        /// <summary>
        /// Conversor para TimeOnly
        /// </summary>
        public static readonly ValueConverter<TimeOnly, TimeSpan> TimeOnlyConverter = 
            new ValueConverter<TimeOnly, TimeSpan>(
                v => v.ToTimeSpan(),
                v => TimeOnly.FromTimeSpan(v));

        /// <summary>
        /// Conversor para TimeOnly nullable
        /// </summary>
        public static readonly ValueConverter<TimeOnly?, TimeSpan?> NullableTimeOnlyConverter = 
            new ValueConverter<TimeOnly?, TimeSpan?>(
                v => v == null ? null : v.Value.ToTimeSpan(),
                v => v == null ? null : TimeOnly.FromTimeSpan(v.Value));
    }

    /// <summary>
    /// Classe conversor específica para DateOnly
    /// </summary>
    public class DateOnlyToDateTimeConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyToDateTimeConverter() 
            : base(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
        {
        }
    }

    /// <summary>
    /// Classe conversor específica para DateOnly nullable
    /// </summary>
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