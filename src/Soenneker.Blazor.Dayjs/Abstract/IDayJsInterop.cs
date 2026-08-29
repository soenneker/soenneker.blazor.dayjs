using System;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.Dayjs.Configuration;
using Soenneker.Blazor.Dayjs.Dtos;

namespace Soenneker.Blazor.Dayjs.Abstract;

/// <summary>
/// A Blazor interop library for Day.js
/// </summary>
public interface IDayJsInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes the day javascript so it is ready for use.
    /// </summary>
    /// <param name="useCdn">Whether cdn.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the day javascript is ready for use.</returns>
    ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initializes the day javascript so it is ready for use.
    /// </summary>
    /// <param name="options">Options to configure for the day javascript.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task that completes when the day javascript is ready for use.</returns>
    ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats the supplied date as relative time measured from now.
    /// </summary>
    /// <param name="value">Date and time used to calculate the relative-time text.</param>
    /// <param name="withoutSuffix">Whether to omit the relative-time suffix.</param>
    /// <param name="timezone">Time zone used when formatting the date.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by from Now.</returns>
    ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats the supplied date as relative time measured to now.
    /// </summary>
    /// <param name="value">Date and time used to calculate the relative-time text.</param>
    /// <param name="withoutSuffix">Whether to omit the relative-time suffix.</param>
    /// <param name="timezone">Time zone used when formatting the date.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by to Now.</returns>
    ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Formats a duration as human-readable relative-time text.
    /// </summary>
    /// <param name="duration">Duration for the duration humanize operation.</param>
    /// <param name="withoutSuffix">Whether to omit the relative-time suffix.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by duration Humanize.</returns>
    ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a subscription that periodically reports updated relative-time text.
    /// </summary>
    /// <param name="value">Date and time used to calculate the relative-time text.</param>
    /// <param name="updateInterval">Update Interval for the subscribe relative operation.</param>
    /// <param name="onUpdate">Callback used by subscribe relative.</param>
    /// <param name="withoutSuffix">Whether to omit the relative-time suffix.</param>
    /// <param name="timezone">Time zone used when formatting the date.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested day JavaScript Subscription.</returns>
    ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default);
}
