[![](https://img.shields.io/nuget/v/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.dayjs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.dayjs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.dayjs)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.dayjs/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.dayjs/actions/workflows/codeql.yml)

# Soenneker.Blazor.Dayjs

A Blazor wrapper around [Day.js](https://day.js.org/) for relative timestamps and humanized durations, with Razor components for live clocks and countdowns.

## Installation

```bash
dotnet add package Soenneker.Blazor.Dayjs
```

## Setup

```csharp
using Soenneker.Blazor.Dayjs.Registrars;

builder.Services.AddDayJsInteropAsScoped();
```

Initialize the plugins your application uses before rendering a relative-time component or calling the interop service. A layout or other parent component is a convenient place:

```razor
@using Soenneker.Blazor.Dayjs.Abstract
@using Soenneker.Blazor.Dayjs.Configuration
@inject IDayJsInterop DayJs

@code {
    protected override async Task OnInitializedAsync()
    {
        await DayJs.Initialize(new DayJsOptions
        {
            LoadRelativeTime = true,
            LoadDuration = true
        });
    }
}
```

`UseCdn` defaults to `true`. Set it to `false` to load the Day.js files packaged with this library instead of jsDelivr. Initialization is shared by the scoped service, so choose the complete plugin set on the first call.

## Components

Add the component namespace to `_Imports.razor`:

```razor
@using Soenneker.Blazor.Dayjs
```

### Relative time

```razor
<DayJsRelative Value="article.PublishedAt"
               UpdateInterval="30s" />
```

This renders values such as `5 minutes ago`. It requires `LoadRelativeTime`. Set `WithoutSuffix="true"` for text such as `5 minutes`, or `AutomaticUpdate="false"` for a single calculation.

### Live clock

```razor
<DayJsNow Format="HH:mm:ss" UpdateInterval="1s" />
```

`Format` is a .NET date/time format string. `Timezone` is resolved by .NET's `TimeZoneInfo`; if omitted, the process/browser local time is used.

### Countdown

```razor
<DayJsUntil Value="eventStartsAt"
            Format="d.hh:mm:ss"
            UpdateInterval="1s" />
```

`Format` is a custom .NET `TimeSpan` format; colons are escaped for you. Expired countdowns render zero by default. Set `ClampToZero="false"` to keep displaying a negative duration.

Intervals accept a `TimeSpan` value or a positive number followed by `ms`, `s`, `m`, `h`, or `d`. Invalid and non-positive component intervals fall back to that component's default.

## Interop API

Inject `IDayJsInterop` when you need the value rather than rendered text:

```csharp
string age = await DayJs.FromNow(article.PublishedAt);
string duration = await DayJs.DurationHumanize(TimeSpan.FromMinutes(90));
```

`FromNow`, `ToNow`, and `SubscribeRelative` require `LoadRelativeTime`. `DurationHumanize` requires `LoadDuration`. To pass an IANA timezone such as `America/Chicago` to the relative-time methods, enable both `LoadUtc` and `LoadTimezone`.

`SubscribeRelative` returns an `IAsyncDisposable`; retain and dispose it when its owner is disposed:

```csharp
DayJsSubscription subscription = await DayJs.SubscribeRelative(
    article.PublishedAt,
    TimeSpan.FromMinutes(1),
    value => relativeText = value);

await subscription.DisposeAsync();
```
