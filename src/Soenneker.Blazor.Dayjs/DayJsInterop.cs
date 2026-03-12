using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Blazor.Dayjs.Configuration;
using Soenneker.Blazor.Dayjs.Dtos;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;

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

    private readonly CancellationScope _cancellationScope = new();

    public DayJsInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;
        _scriptInitializer = new AsyncInitializer<DayJsOptions>(InitializeScript);
    }

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _options = new DayJsOptions { UseCdn = useCdn };

        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    public async ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default)
    {
        _options = options ?? new DayJsOptions();

        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    private async ValueTask EnsureInitialized(CancellationToken cancellationToken)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    private async ValueTask InitializeScript(DayJsOptions options, CancellationToken token)
    {
        if (options.UseCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(
                "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/dayjs.min.js",
                "dayjs",
                integrity: "sha256-nP25Pzivzy0Har7NZtMr/TODzfGWdlTrwmomYF2vQXM=",
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
                    integrity: "sha256-qChvIvJkeTbV5m0C0KrBl+sOicaIklgEk82lwpDSkZE=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }

            if (options.LoadRelativeTime)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/relativeTime.js",
                    "dayjs_plugin_relativeTime",
                    integrity: "sha256-muryXOPFkVJcJO1YFmhuKyXYmGDT2TYVxivG0MCgRzg=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }

            if (options.LoadDuration)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/duration.js",
                    "dayjs_plugin_duration",
                    integrity: "sha256-GV/dpEfJoONuuRAyFBHaj2U7CnzhUYJLXAX4zJybJFA=",
                    crossOrigin: "anonymous",
                    cancellationToken: token);
            }
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(
                "_content/Soenneker.Blazor.Dayjs/js/dayjs.min.js",
                "dayjs",
                cancellationToken: token);

            if (options.LoadUtc)
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Dayjs/js/utc.js",
                    "dayjs_plugin_utc",
                    cancellationToken: token);

            if (options.LoadTimezone)
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Dayjs/js/timezone.js",
                    "dayjs_plugin_timezone",
                    cancellationToken: token);

            if (options.LoadRelativeTime)
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Dayjs/js/relativeTime.js",
                    "dayjs_plugin_relativeTime",
                    cancellationToken: token);

            if (options.LoadDuration)
                await _resourceLoader.LoadScriptAndWaitForVariable(
                    "_content/Soenneker.Blazor.Dayjs/js/duration.js",
                    "dayjs_plugin_duration",
                    cancellationToken: token);
        }

        await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);
    }

    public async ValueTask<string> FromNow(
        DateTimeOffset value,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await EnsureInitialized(linked);
            return await _jsRuntime.InvokeAsync<string>("DayJsInterop.fromNow", linked, value, withoutSuffix, timezone);
        }
    }

    public async ValueTask<string> ToNow(
        DateTimeOffset value,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await EnsureInitialized(linked);
            return await _jsRuntime.InvokeAsync<string>("DayJsInterop.toNow", linked, value, withoutSuffix, timezone);
        }
    }

    public async ValueTask<string> DurationHumanize(
        TimeSpan duration,
        bool withoutSuffix = false,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await EnsureInitialized(linked);
            return await _jsRuntime.InvokeAsync<string>(
                "DayJsInterop.durationHumanize",
                linked,
                duration.TotalMilliseconds,
                withoutSuffix);
        }
    }

    public async ValueTask<DayJsSubscription> SubscribeRelative(
        DateTimeOffset value,
        TimeSpan updateInterval,
        Action<string> onUpdate,
        bool withoutSuffix = false,
        string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await EnsureInitialized(linked);

            var callback = new DayJsUpdateCallback(onUpdate);
            var dotNetRef = DotNetObjectReference.Create(callback);

            var id = await _jsRuntime.InvokeAsync<long>(
                "DayJsInterop.subscribeRelative",
                linked,
                value,
                updateInterval.TotalMilliseconds,
                withoutSuffix,
                timezone,
                dotNetRef);

            return new DayJsSubscription(_jsRuntime, id, dotNetRef);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
