[![](https://img.shields.io/nuget/v/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.blazor.dayjs/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.blazor.dayjs/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.blazor.dayjs.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.blazor.dayjs/)
[![](https://img.shields.io/badge/Demo-Live-blueviolet?style=for-the-badge&logo=github)](https://soenneker.github.io/soenneker.blazor.dayjs)

# Soenneker.Blazor.Dayjs
### Blazor interop for Day.js

A lightweight Blazor interop library for [Day.js](https://day.js.org/) focused on **live, auto-updating time displays**.

---

## Installation

```bash
dotnet add package Soenneker.Blazor.Dayjs
````

---

## Setup

Register the service:

```csharp
builder.Services.AddDayJsInteropAsScoped();
```

Add a using:

```razor
@using Soenneker.Blazor.Dayjs
```

---

## Components

### Live relative time

```razor
<DayJsRelative Value="CreatedAt" UpdateInterval="1m" />
```

---

### Live clock

Uses .NET date/time format strings.

```razor
<DayJsNow Format="HH:mm:ss" UpdateInterval="1s" />
```

---

### Countdown / time until

Uses .NET `TimeSpan` format strings.

```razor
<DayJsUntil Value="LaunchTime" Format="mm:ss" UpdateInterval="1s" />
```

---

## Configuration

Day.js plugins are loaded explicitly.

```razor
@using Soenneker.Blazor.Dayjs.Configuration
@inject IDayJsInterop DayJsInterop
```

```csharp
await DayJsInterop.Initialize(new DayJsOptions
{
    UseCdn = true,

    LoadRelativeTime = true,
    LoadTimezone     = true,
    LoadUtc          = true,
    LoadDuration     = true
});
```