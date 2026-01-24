[![](https://img.shields.io/nuget/v/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.dayjs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.dayjs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Blazor.Dayjs
### A Blazor interop library for Day.js

## Installation

```
dotnet add package Soenneker.Blazor.Dayjs
```

## Setup

Day.js core is loaded automatically via CDN by default. Plugins are only loaded when enabled via options.
If you want to self-host, place the scripts at:

```
_content/Soenneker.Blazor.Dayjs/js/dayjs.min.js
_content/Soenneker.Blazor.Dayjs/js/utc.js
_content/Soenneker.Blazor.Dayjs/js/timezone.js
_content/Soenneker.Blazor.Dayjs/js/relativeTime.js
_content/Soenneker.Blazor.Dayjs/js/duration.js
```

Register the service:

```
builder.Services.AddDayJsInteropAsScoped();
```

Add a using:

```
@using Soenneker.Blazor.Dayjs
```

Optional init to control CDN usage:

```
@inject IDayJsInterop DayJsInterop

await DayJsInterop.Initialize(useCdn: true);
```

Enable plugins explicitly:

```
@using Soenneker.Blazor.Dayjs.Configuration
@inject IDayJsInterop DayJsInterop

await DayJsInterop.Initialize(new DayJsOptions
{
    UseCdn = true,
    LoadRelativeTime = true,
    LoadTimezone = true,
    LoadUtc = true,
    LoadDuration = true
});
```

## Components

Live relative time:

```
<DayJsRelative Value="CreatedAt" UpdateInterval="1m" />
```

Live clock:

```
<DayJsNow Format="HH:mm:ss" Timezone="America/Chicago" UpdateInterval="1s" />
```

Countdown:

```
<DayJsUntil Value="LaunchTime" Format="mm:ss" UpdateInterval="1s" />
```

## Manual subscriptions

```
await using var sub = await DayJsInterop.SubscribeRelative(
    value: CreatedAt,
    updateInterval: TimeSpan.FromSeconds(30),
    onUpdate: text => Console.WriteLine(text));
```
