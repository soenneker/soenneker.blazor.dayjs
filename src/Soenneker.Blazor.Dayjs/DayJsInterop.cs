using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Blazor.Dayjs.Configuration;
using Soenneker.Blazor.Dayjs.Dtos;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Utils.CancellationScopes;

namespace Soenneker.Blazor.Dayjs;

/// <inheritdoc cref="IDayJsInterop"/>
public sealed class DayJsInterop : IDayJsInterop
{
    private const string _modulePath = "_content/Soenneker.Blazor.Dayjs/js/dayjsinterop.js";

    private readonly IResourceLoader _resourceLoader;
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly AsyncInitializer<DayJsOptions> _scriptInitializer;
    private DayJsOptions _options = new();

    private readonly CancellationScope _cancellationScope = new();

    public DayJsInterop(IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;
        _scriptInitializer = new AsyncInitializer<DayJsOptions>(InitializeScript);
    }

    private async ValueTask EnsureGlobalScript(CancellationToken token, string uri, string globalName, string? integrity = null,
        string? crossOrigin = "anonymous", bool loadInHead = false, bool scriptAsync = false, bool scriptDefer = false, int delay = 16, int? timeout = null)
    {
        await _resourceLoader.LoadScriptAndWaitForVariable(uri, globalName, integrity, crossOrigin, loadInHead, scriptAsync, scriptDefer,
            delay, timeout, token);
    }

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _options = new DayJsOptions { UseCdn = useCdn };

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    public async ValueTask Initialize(DayJsOptions options, CancellationToken cancellationToken = default)
    {
        _options = options ?? new DayJsOptions();

        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    private async ValueTask EnsureInitialized(CancellationToken cancellationToken)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
            await _scriptInitializer.Init(_options, linked);
    }

    private async ValueTask InitializeScript(DayJsOptions options, CancellationToken token)
    {
        if (options.UseCdn)
        {
            await EnsureGlobalScript(token, "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/dayjs.min.js", "dayjs",
                integrity: "sha256-nP25Pzivzy0Har7NZtMr/TODzfGWdlTrwmomYF2vQXM=", crossOrigin: "anonymous");

            if (options.LoadUtc)
            {
                await EnsureGlobalScript(token, "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/utc.js", "dayjs_plugin_utc",
                    integrity: "sha256-fgEHLm8fLmRlBqCDzMUA7RnFplelpmRbP6/szfPOnAo=", crossOrigin: "anonymous");
            }

            if (options.LoadTimezone)
            {
                await EnsureGlobalScript(token, "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/timezone.js", "dayjs_plugin_timezone",
                    integrity: "sha256-qChvIvJkeTbV5m0C0KrBl+sOicaIklgEk82lwpDSkZE=", crossOrigin: "anonymous");
            }

            if (options.LoadRelativeTime)
            {
                await EnsureGlobalScript(token, "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/relativeTime.js", "dayjs_plugin_relativeTime",
                    integrity: "sha256-muryXOPFkVJcJO1YFmhuKyXYmGDT2TYVxivG0MCgRzg=", crossOrigin: "anonymous");
            }

            if (options.LoadDuration)
            {
                await EnsureGlobalScript(token, "https://cdn.jsdelivr.net/npm/dayjs@1.11.19/plugin/duration.js", "dayjs_plugin_duration",
                    integrity: "sha256-GV/dpEfJoONuuRAyFBHaj2U7CnzhUYJLXAX4zJybJFA=", crossOrigin: "anonymous");
            }
        }
        else
        {
            await EnsureGlobalScript(token, "_content/Soenneker.Blazor.Dayjs/js/dayjs.min.js", "dayjs");

            if (options.LoadUtc)
                await EnsureGlobalScript(token, "_content/Soenneker.Blazor.Dayjs/js/utc.js", "dayjs_plugin_utc");

            if (options.LoadTimezone)
                await EnsureGlobalScript(token, "_content/Soenneker.Blazor.Dayjs/js/timezone.js", "dayjs_plugin_timezone");

            if (options.LoadRelativeTime)
                await EnsureGlobalScript(token, "_content/Soenneker.Blazor.Dayjs/js/relativeTime.js", "dayjs_plugin_relativeTime");

            if (options.LoadDuration)
                await EnsureGlobalScript(token, "_content/Soenneker.Blazor.Dayjs/js/duration.js", "dayjs_plugin_duration");
        }

        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, token);
    }

    public async ValueTask<string> FromNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await EnsureInitialized(linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            return await module.InvokeAsync<string>("fromNow", linked, value, withoutSuffix, timezone);
        }
    }

    public async ValueTask<string> ToNow(DateTimeOffset value, bool withoutSuffix = false, string? timezone = null,
        CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await EnsureInitialized(linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            return await module.InvokeAsync<string>("toNow", linked, value, withoutSuffix, timezone);
        }
    }

    public async ValueTask<string> DurationHumanize(TimeSpan duration, bool withoutSuffix = false, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await EnsureInitialized(linked);
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            return await module.InvokeAsync<string>("durationHumanize", linked, duration.TotalMilliseconds, withoutSuffix);
        }
    }

    public async ValueTask<DayJsSubscription> SubscribeRelative(DateTimeOffset value, TimeSpan updateInterval, Action<string> onUpdate,
        bool withoutSuffix = false, string? timezone = null, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await EnsureInitialized(linked);

            var callback = new DayJsUpdateCallback(onUpdate);
            DotNetObjectReference<DayJsUpdateCallback> dotNetRef = DotNetObjectReference.Create(callback);

            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, linked);
            var id = await module.InvokeAsync<long>("subscribeRelative", linked, value, updateInterval.TotalMilliseconds, withoutSuffix, timezone, dotNetRef);

            return new DayJsSubscription(_moduleImportUtil, id, dotNetRef);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
