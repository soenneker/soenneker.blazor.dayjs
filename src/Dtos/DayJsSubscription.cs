using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Blazor.Dayjs.Dtos;

public sealed class DayJsSubscription : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly long _id;
    private readonly IDisposable _dotNetReference;
    private bool _disposed;

    internal DayJsSubscription(IJSRuntime jsRuntime, long id, IDisposable dotNetReference)
    {
        _jsRuntime = jsRuntime;
        _id = id;
        _dotNetReference = dotNetReference;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            await _jsRuntime.InvokeVoidAsync("DayJsInterop.unsubscribe", _id);
        }
        finally
        {
            _dotNetReference.Dispose();
        }
    }
}
