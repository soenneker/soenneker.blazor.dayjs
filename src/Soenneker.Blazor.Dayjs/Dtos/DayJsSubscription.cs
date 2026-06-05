using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;

namespace Soenneker.Blazor.Dayjs.Dtos;

/// <summary>
/// Represents the day js subscription.
/// </summary>
public sealed class DayJsSubscription : IAsyncDisposable
{
    private const string _modulePath = "_content/Soenneker.Blazor.Dayjs/js/dayjsinterop.js";
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly long _id;
    private readonly IDisposable _dotNetReference;
    private bool _disposed;

    internal DayJsSubscription(IModuleImportUtil moduleImportUtil, long id, IDisposable dotNetReference)
    {
        _moduleImportUtil = moduleImportUtil;
        _id = id;
        _dotNetReference = dotNetReference;
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            IJSObjectReference module = await _moduleImportUtil.GetContentModuleReference(_modulePath, default);
            await module.InvokeVoidAsync("unsubscribe", _id);
        }
        finally
        {
            _dotNetReference.Dispose();
        }
    }
}
