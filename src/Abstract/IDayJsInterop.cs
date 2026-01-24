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

    ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default);

    ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default);

    ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default);
}
