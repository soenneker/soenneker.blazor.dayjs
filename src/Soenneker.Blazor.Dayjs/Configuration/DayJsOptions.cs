namespace Soenneker.Blazor.Dayjs.Configuration;

/// <summary>
/// Controls Day.js script loading and plugin availability.
/// </summary>
public sealed class DayJsOptions
{
    public bool UseCdn { get; set; } = true;

    public bool LoadUtc { get; set; }

    public bool LoadTimezone { get; set; }

    public bool LoadRelativeTime { get; set; }

    public bool LoadDuration { get; set; }
}