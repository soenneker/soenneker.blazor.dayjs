using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Blazor.Dayjs.Configuration;
using Soenneker.Blazor.Dayjs.Dtos;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;

namespace Soenneker.Blazor.Dayjs;

/// <inheritdoc cref="IDayJsInterop"/>
public sealed class DayJsInterop : IDayJsInterop
{
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer<DayJsOptions> _scriptInitializer;
    private DayJsOptions _options = new();

    private const string _module = "Soenneker.Blazor.Dayjs/js/dayjsinterop.js";
    private const string _moduleName = "DayJsInterop";

    private readonly IJSRuntime _jsRuntime;

    public DayJsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;
        _scriptInitializer = new AsyncInitializer<DayJsOptions>(InitializeScript);
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _options = new DayJsOptions { UseCdn = useCdn };
        return _scriptInitializer.Init(_options, cancellationToken);
    }

    public ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default)
    {
        _options = options ?? new DayJsOptions();
        return _scriptInitializer.Init(_options, cancellationToken);
    }

    public async ValueTask<string> Format(DateTimeOffset value, string format, string? timezone = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.format", cancellationToken, value, format, timezone);
    }

    public async ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.fromNow", cancellationToken, value, withoutSuffix, timezone);
    }

    public async ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.toNow", cancellationToken, value, withoutSuffix, timezone);
    }

    public async ValueTask<string> Add(DateTimeOffset value, TimeSpan amount, string format, string? timezone = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.add", cancellationToken, value, amount.TotalMilliseconds, format, timezone);
    }

    public async ValueTask<string> Subtract(DateTimeOffset value, TimeSpan amount, string format, string? timezone = null, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.subtract", cancellationToken, value, amount.TotalMilliseconds, format, timezone);
    }

    public async ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.durationHumanize", cancellationToken, duration.TotalMilliseconds, withoutSuffix);
    }

    public async ValueTask<string> Until(DateTimeOffset value, string format, string? timezone = null, bool clampToZero = true, CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        return await _jsRuntime.InvokeAsync<string>("DayJsInterop.until", cancellationToken, value, format, timezone, clampToZero);
    }

    public async ValueTask<DayJsSubscription> SubscribeNow(
        string format,
        string? timezone,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        var callback = new DayJsUpdateCallback(onUpdate);
        var dotNetRef = DotNetObjectReference.Create(callback);

        var id = await _jsRuntime.InvokeAsync<long>(
            "DayJsInterop.subscribeNow",
            cancellationToken,
            format,
            timezone,
            updateInterval.TotalMilliseconds,
            dotNetRef);

        return new DayJsSubscription(_jsRuntime, id, dotNetRef);
    }

    public async ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        var callback = new DayJsUpdateCallback(onUpdate);
        var dotNetRef = DotNetObjectReference.Create(callback);

        var id = await _jsRuntime.InvokeAsync<long>(
            "DayJsInterop.subscribeRelative",
            cancellationToken,
            value,
            updateInterval.TotalMilliseconds,
            withoutSuffix,
            timezone,
            dotNetRef);

        return new DayJsSubscription(_jsRuntime, id, dotNetRef);
    }

    public async ValueTask<DayJsSubscription> SubscribeUntil(
        DateTimeOffset value,
        string format,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        string? timezone = null,
        bool clampToZero = true,
        CancellationToken cancellationToken = default)
    {
        await EnsureInitialized(cancellationToken);
        var callback = new DayJsUpdateCallback(onUpdate);
        var dotNetRef = DotNetObjectReference.Create(callback);

        var id = await _jsRuntime.InvokeAsync<long>(
            "DayJsInterop.subscribeUntil",
            cancellationToken,
            value,
            format,
            updateInterval.TotalMilliseconds,
            timezone,
            clampToZero,
            dotNetRef);

        return new DayJsSubscription(_jsRuntime, id, dotNetRef);
    }

    private ValueTask EnsureInitialized(CancellationToken cancellationToken)
    {
        return _scriptInitializer.Init(_options, cancellationToken);
    }

    private async ValueTask InitializeScript(DayJsOptions options, CancellationToken token)
    {
        if (options.UseCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(
                "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/dayjs.min.js",
                "dayjs",
                integrity: "sha256-S3rLdz1b38wy5uE3m8AvkhlKoYcXW8BXTa54OD3L32s=",
                crossOrigin: "anonymous",
                cancellationToken: token);

            if (options.LoadUtc)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/utc.js",
                    "dayjs_plugin_utc",
                    integrity: "sha256-fgEHLm8fLmRlBqCDzMUA7RnFplelpmRbP6/szfPOnAo=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }

            if (options.LoadTimezone)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/timezone.js",
                    "dayjs_plugin_timezone",
                    integrity: "sha256-5X2s7ZrO2sQY4z7n0sZK1aQ7C8yU+z7F5xg0+3q1kKk=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }

            if (options.LoadRelativeTime)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/relativeTime.js",
                    "dayjs_plugin_relativeTime",
                    integrity: "sha256-Jo2Y7gP7eFJ9y4v8m4bNwJ3U4+1r2n6RZxQ4ZlZ6xE0=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }

            if (options.LoadDuration)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/duration.js",
                    "dayjs_plugin_duration",
                    integrity: "sha256-gP9N4nH9y7qzZ4nC6lFz0m5bY0P0g8m5K0wM2Q7N9b8=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }
            else
            {
                await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.Dayjs/js/dayjs.min.js", "dayjs", cancellationToken: token);

                if (options.LoadUtc)
                    await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.Dayjs/js/utc.js", "dayjs_plugin_utc", cancellationToken: token);

                if (options.LoadTimezone)
                    await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.Dayjs/js/timezone.js", "dayjs_plugin_timezone", cancellationToken: token);

                if (options.LoadRelativeTime)
                    await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.Dayjs/js/relativeTime.js", "dayjs_plugin_relativeTime", cancellationToken: token);

                if (options.LoadDuration)
                    await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Blazor.Dayjs/js/duration.js", "dayjs_plugin_duration", cancellationToken: token);
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
    }
}
