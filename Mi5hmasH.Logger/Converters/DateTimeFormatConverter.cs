using System.ComponentModel;
using System.Globalization;

namespace Mi5hmasH.Logger.Converters;

/// <summary>
/// Provides conversion between <see cref="DateTime"/> objects and their string representations using a configurable
/// date and time format. Supports custom formatting via the <see cref="DateTimeFormatAttribute"/> applied to
/// properties.
/// </summary>
public class DateTimeFormatConverter : TypeConverter
{
    private static string GetFormat(ITypeDescriptorContext? context)
    {
        var formatAttr = context?.PropertyDescriptor?.Attributes[typeof(DateTimeFormatAttribute)] as DateTimeFormatAttribute;
        return formatAttr?.Format ?? "yyyy-MM-dd HH:mm:ss.fff";
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var format = GetFormat(context);
        if (value is string s && DateTime.TryParseExact(s, format, culture, DateTimeStyles.None, out var dt))
            return dt;
        return base.ConvertFrom(context, culture, value);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        var format = GetFormat(context);
        if (destinationType == typeof(string) && value is DateTime dt)
            return dt.ToString(format, culture);
        return base.ConvertTo(context, culture, value, destinationType);
    }
}

/// <summary>
/// Specifies the date and time format string to use when serializing or deserializing a property.
/// </summary>
/// <param name="format">The format string that defines how the date and time value should be represented.</param>
[AttributeUsage(AttributeTargets.Property)]
public class DateTimeFormatAttribute(string format) : Attribute
{
    public string Format { get; } = format;
}