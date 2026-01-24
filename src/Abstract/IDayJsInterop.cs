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
    ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default);

    ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default);

    ValueTask<string> Format(DateTimeOffset value, string format, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> Add(DateTimeOffset value, TimeSpan amount, string format, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> Subtract(DateTimeOffset value, TimeSpan amount, string format, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default);

    ValueTask<string> Until(DateTimeOffset value, string format, string? timezone = null, bool clampToZero = true, CancellationToken cancellationToken = default);

    ValueTask<DayJsSubscription> SubscribeNow(
        string format,
        string? timezone,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        CancellationToken cancellationToken = default);

    ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default);

    ValueTask<DayJsSubscription> SubscribeUntil(
        DateTimeOffset value,
        string format,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        string? timezone = null,
        bool clampToZero = true,
        CancellationToken cancellationToken = default);
}
