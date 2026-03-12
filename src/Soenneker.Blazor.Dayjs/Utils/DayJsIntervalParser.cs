using System;
using System.Globalization;

namespace Soenneker.Blazor.Dayjs.Utils;

public static class DayJsIntervalParser
{
    public static TimeSpan ParseOrDefault(string? value, TimeSpan fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        ReadOnlySpan<char> trimmed = value.AsSpan().Trim();

        if (trimmed.IsEmpty)
            return fallback;

        if (TimeSpan.TryParse(trimmed, CultureInfo.InvariantCulture, out var parsed))
            return Normalize(parsed, fallback);

        if (TryParseWithUnit(trimmed, out var withUnit))
            return Normalize(withUnit, fallback);

        return fallback;
    }

    private static TimeSpan Normalize(TimeSpan value, TimeSpan fallback)
    {
        if (value <= TimeSpan.Zero)
            return fallback;

        return value;
    }

    private static bool TryParseWithUnit(ReadOnlySpan<char> value, out TimeSpan result)
    {
        result = default;

        ReadOnlySpan<char> numberPart;
        char unit;

        if (value.EndsWith("ms", StringComparison.OrdinalIgnoreCase))
        {
            numberPart = value[..^2];
            unit = 'M';
        }
        else
        {
            if (value.Length < 2)
                return false;

            numberPart = value[..^1];
            unit = value[^1];
        }

        if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            return false;

        if (number <= 0)
            return false;

        result = unit switch
        {
            'M' => TimeSpan.FromMilliseconds(number),
            's' or 'S' => TimeSpan.FromSeconds(number),
            'm' => TimeSpan.FromMinutes(number),
            'h' or 'H' => TimeSpan.FromHours(number),
            'd' or 'D' => TimeSpan.FromDays(number),
            _ => default
        };

        return result != default;
    }
}
