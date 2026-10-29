using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Soenneker.Blazor.Dayjs.Dtos;

namespace Soenneker.Blazor.Dayjs.Tests;

public sealed class DayJsSubscriptionTests
{
    [Test]
    public async Task Dispose_unsubscribes_once_and_releases_callback()
    {
        var module = new SubscriptionModuleStub();
        var callback = new SubscriptionCallbackStub();
        var subscription = new DayJsSubscription(new SubscriptionModuleImportStub(module), 42, callback);
        await subscription.DisposeAsync();
        await subscription.DisposeAsync();
        await Assert.That(module.Calls).IsEqualTo(1);
        await Assert.That(module.Identifier).IsEqualTo("unsubscribe");
        await Assert.That((long)module.Arguments![0]!).IsEqualTo(42L);
        await Assert.That(callback.IsDisposed).IsTrue();
    }

    [Test]
    public async Task Disconnect_during_unsubscribe_still_releases_callback()
    {
        var module = new SubscriptionModuleStub { Failure = new JSDisconnectedException("Circuit closed") };
        var callback = new SubscriptionCallbackStub();
        var subscription = new DayJsSubscription(new SubscriptionModuleImportStub(module), 42, callback);
        await subscription.DisposeAsync();
        await Assert.That(callback.IsDisposed).IsTrue();
    }

    [Test]
    public async Task Script_failure_retains_cause_and_releases_callback()
    {
        var failure = new JSException("unsubscribe failed");
        var module = new SubscriptionModuleStub { Failure = failure };
        var callback = new SubscriptionCallbackStub();
        var subscription = new DayJsSubscription(new SubscriptionModuleImportStub(module), 42, callback);
        try
        {
            await subscription.DisposeAsync();
        }
        catch (JSException exception)
        {
            await Assert.That(ReferenceEquals(exception.InnerException, failure)).IsTrue();
            await Assert.That(exception.Message.Contains("DayJsSubscription.DisposeAsync", StringComparison.Ordinal)).IsTrue();
            await Assert.That(callback.IsDisposed).IsTrue();
            return;
        }
        throw new InvalidOperationException("Script failures must propagate.");
    }

    [Test]
    public async Task Cancellation_propagates_unchanged_and_releases_callback()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var failure = new OperationCanceledException(source.Token);
        var module = new SubscriptionModuleStub { Failure = failure };
        var callback = new SubscriptionCallbackStub();
        var subscription = new DayJsSubscription(new SubscriptionModuleImportStub(module), 42, callback);
        try
        {
            await subscription.DisposeAsync();
        }
        catch (OperationCanceledException exception)
        {
            await Assert.That(ReferenceEquals(exception, failure)).IsTrue();
            await Assert.That(callback.IsDisposed).IsTrue();
            return;
        }
        throw new InvalidOperationException("Cancellation must propagate.");
    }
}
