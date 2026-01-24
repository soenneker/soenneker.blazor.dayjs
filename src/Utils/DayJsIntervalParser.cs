using System;
using System.Globalization;

namespace Soenneker.Blazor.Dayjs.Utils;

public static class DayJsIntervalParser
{
    public static TimeSpan ParseOrDefault(string? value, TimeSpan fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback;

        var trimmed = value.Trim();

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

    private static bool TryParseWithUnit(string value, out TimeSpan result)
    {
        result = default;

        var lower = value.ToLowerInvariant();

        string numberPart;
        string unit;

        if (lower.EndsWith("ms", StringComparison.Ordinal))
        {
            numberPart = lower[..^2];
            unit = "ms";
        }
        else
        {
            if (lower.Length < 2)
                return false;

            numberPart = lower[..^1];
            unit = lower[^1].ToString();
        }

        if (!double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            return false;

        if (number <= 0)
            return false;

        result = unit switch
        {
            "ms" => TimeSpan.FromMilliseconds(number),
            "s" => TimeSpan.FromSeconds(number),
            "m" => TimeSpan.FromMinutes(number),
            "h" => TimeSpan.FromHours(number),
            "d" => TimeSpan.FromDays(number),
            _ => default
        };

        return result != default;
    }
}
