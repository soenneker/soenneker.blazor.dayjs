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
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="useCdn">The use cdn.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the initialize operation.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the from now operation.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="withoutSuffix">The without suffix.</param>
    /// <param name="timezone">The timezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the to now operation.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="withoutSuffix">The without suffix.</param>
    /// <param name="timezone">The timezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the duration humanize operation.
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <param name="withoutSuffix">The without suffix.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the subscribe relative operation.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="updateInterval">The update interval.</param>
    /// <param name="onUpdate">The on update.</param>
    /// <param name="withoutSuffix">The without suffix.</param>
    /// <param name="timezone">The timezone.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default);
}
