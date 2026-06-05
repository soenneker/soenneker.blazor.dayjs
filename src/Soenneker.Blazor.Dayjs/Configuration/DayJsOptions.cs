namespace Soenneker.Blazor.Dayjs.Configuration;

/// <summary>
/// Controls Day.js script loading and plugin availability.
/// </summary>
public sealed class DayJsOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether use cdn.
    /// </summary>
    public bool UseCdn { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether load utc.
    /// </summary>
    public bool LoadUtc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether load timezone.
    /// </summary>
    public bool LoadTimezone { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether load relative time.
    /// </summary>
    public bool LoadRelativeTime { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether load duration.
    /// </summary>
    public bool LoadDuration { get; set; }
}