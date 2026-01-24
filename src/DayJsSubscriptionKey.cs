using System;

namespace Soenneker.Blazor.Dayjs;

internal readonly struct DayJsSubscriptionKey : IEquatable<DayJsSubscriptionKey>
{
    private readonly long _value;
    private readonly string _format;
    private readonly string? _updateInterval;
    private readonly string? _timezone;
    private readonly bool _withoutSuffix;
    private readonly bool _clampToZero;
    private readonly bool _automaticUpdate;

    public DayJsSubscriptionKey(
        long value,
        string format,
        string? updateInterval,
        string? timezone,
        bool withoutSuffix,
        bool clampToZero,
        bool automaticUpdate)
    {
        _value = value;
        _format = format;
        _updateInterval = updateInterval;
        _timezone = timezone;
        _withoutSuffix = withoutSuffix;
        _clampToZero = clampToZero;
        _automaticUpdate = automaticUpdate;
    }

    public bool Equals(DayJsSubscriptionKey other)
    {
        return _value == other._value
            && string.Equals(_format, other._format, StringComparison.Ordinal)
            && string.Equals(_updateInterval, other._updateInterval, StringComparison.Ordinal)
            && string.Equals(_timezone, other._timezone, StringComparison.Ordinal)
            && _withoutSuffix == other._withoutSuffix
            && _clampToZero == other._clampToZero
            && _automaticUpdate == other._automaticUpdate;
    }

    public override bool Equals(object? obj)
    {
        return obj is DayJsSubscriptionKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            _value,
            _format,
            _updateInterval is null ? 0 : StringComparer.Ordinal.GetHashCode(_updateInterval),
            _timezone is null ? 0 : StringComparer.Ordinal.GetHashCode(_timezone),
            _withoutSuffix,
            _clampToZero,
            _automaticUpdate);
    }
}
