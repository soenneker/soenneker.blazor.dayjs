using System;
using Microsoft.JSInterop;

namespace Soenneker.Blazor.Dayjs;

internal sealed class DayJsUpdateCallback
{
    private readonly Action<string> _onUpdate;

    public DayJsUpdateCallback(Action<string> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    [JSInvokable]
    public void OnUpdate(string value)
    {
        _onUpdate(value);
    }
}
