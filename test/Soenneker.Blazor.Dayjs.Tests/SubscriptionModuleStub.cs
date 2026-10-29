using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Soenneker.Blazor.Dayjs.Tests;

internal sealed class SubscriptionModuleStub : IJSObjectReference
{
    public Exception? Failure { get; set; }
    public int Calls { get; private set; }
    public string? Identifier { get; private set; }
    public object?[]? Arguments { get; private set; }

    public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
    {
        Calls++;
        Identifier = identifier;
        Arguments = args;
        await Task.Yield();
        if (Failure is not null)
            throw Failure;
        return default!;
    }

    public async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        Calls++;
        Identifier = identifier;
        Arguments = args;
        await Task.Yield();
        if (Failure is not null)
            throw Failure;
        return default!;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
