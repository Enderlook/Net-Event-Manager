using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolder
{
    public abstract void Purge();

    public abstract bool RemoveIfEmpty([NotNullWhen(true)] out Type? eventType, [NotNullWhen(true)] out Type? holderType);

    public abstract bool WasRemoved();

    public abstract Slice GetCallbacks();

    public abstract void Dispose();

    public abstract void RaiseDerived<TConcreteEvent>(Slice slice, object? argument);
}

internal abstract class InvokersHolder<TEvent> : InvokersHolder
{
    public abstract void Raise(Slice slice, TEvent argument);
}

internal sealed class InvokersHolder<TEvent, TCallbackHelper, TCallback> : InvokersHolder<TEvent>
    where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
{
    private TCallback[]? callbacks = ArrayUtils.InitialArray<TCallback>();
    private int count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe(TCallback callback)
        => ArrayUtils.ConcurrentAdd(ref callbacks, ref count, callback);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TPredicator>(TPredicator predicator)
        where TPredicator : IPredicator<TCallback>
        => ArrayUtils.ConcurrentRemove(ref callbacks, ref count, predicator);

    public override Slice GetCallbacks()
    {
        TCallback[] callbacks_ = Utils.Take(ref callbacks);
        int count_ = count;
        if (Utils.Null<TCallbackHelper>().IsOnce())
        {
            count = 0;
            Utils.Untake(ref callbacks, ArrayUtils.RentArray<TCallback>(count_)); // Alternatively we could do, ArrayUtils.InitialArray<TCallback>();
            return new(callbacks_, count_);
        }
        else
        {
            TCallback[] array = ArrayUtils.RentArray<TCallback>(count_);
            Array.Copy(callbacks_, array, count_);
            Utils.Untake(ref callbacks, callbacks_);
            return new(array, count_);
        }
    }

    public override void Raise(Slice slice, TEvent argument)
    {
        TCallback[] callbacks_ = Utils.ExpectExactType<TCallback[]>(slice.Array);
        Utils.Null<TCallbackHelper>().Invoke(argument, callbacks_, slice.Count);
    }

    public override void RaiseDerived<TConcreteEvent>(Slice slice, object? argument)
    {
        Utils.AssertDerived<TEvent, TConcreteEvent>(argument);
        TCallback[] callbacks_ = Utils.ExpectExactType<TCallback[]>(slice.Array);
        Utils.Null<TCallbackHelper>().Invoke(Utils.ExpectAssignableTypeOrNull<TEvent>(argument), callbacks_, slice.Count);
    }

    public override void Purge()
    {
        TCallback[]? array = callbacks;
        Debug.Assert(array is not null);
        {
            int count_ = count;
            Utils.Null<TCallbackHelper>().Purge(ref array, ref count_);
            count = count_;
        }
        callbacks = array;
    }

    public override bool RemoveIfEmpty([NotNullWhen(true)] out Type? eventType, [NotNullWhen(true)] out Type? holderType)
    {
        if (count == 0)
        {
            TCallback[]? array = callbacks;
            Debug.Assert(array is not null);
            ArrayUtils.ReturnArray(array);
            eventType = typeof(TEvent);
            holderType = typeof(InvokersHolder<TEvent, TCallbackHelper, TCallback>);
            return true;
        }
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out eventType);
        Unsafe.SkipInit(out holderType);
#else
        eventType = holderType = null;
#endif
        return false;
    }

    public override bool WasRemoved() => count == 0;

    public override void Dispose()
    {
        int count_ = count;
        if (count_ == 0)
        {
            TCallback[]? array = callbacks;
            Debug.Assert(array is not null);
            ArrayUtils.ReturnArray(array);
        }
        else
        {
            TCallback[]? array = callbacks;
            Debug.Assert(array is not null);
            Utils.Null<TCallbackHelper>().Dispose(array, count_);
            ArrayUtils.ReturnArray(array, count_);
        }
    }
}
