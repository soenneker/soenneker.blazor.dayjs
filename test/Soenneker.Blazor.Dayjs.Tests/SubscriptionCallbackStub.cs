using System;

namespace Soenneker.Blazor.Dayjs.Tests;

internal sealed class SubscriptionCallbackStub : IDisposable
{
    public bool IsDisposed { get; private set; }
    public void Dispose() => IsDisposed = true;
}
