using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Dayjs.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;

namespace Soenneker.Blazor.Dayjs.Registrars;

/// <summary>
/// A Blazor interop library for Day.js
/// </summary>
public static class DayJsRegistrar
{
    /// <summary>
    /// Adds <see cref="IDayJsInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddDayJsInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped();
        services.TryAddScoped<IDayJsInterop, DayJsInterop>();

        return services;
    }
}
