using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolder
{
    private const int ModeExactEvents = 0;
    private const int ModeAssignableEvents = 1;
    private const int ModePurged = 2;

    private int mode;

    public bool ListenToAssignableEvents
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => mode == ModeAssignableEvents;
    }

    protected InvokersHolder(bool listenAssignableEvents) => mode = listenAssignableEvents ? ModeAssignableEvents : ModeExactEvents;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void SetAsPurged() => mode = ModePurged;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool WasPurged() => mode == ModePurged;

    public abstract bool Purge(out InvokersHolderTypeKey holderType, int currentMilliseconds, int purgeMilliseconds, bool hasHightMemoryPressure);

    public abstract Slice GetCallbacks();

    public abstract void Clear(Slice slice);

    public abstract void Dispose();

    public abstract void RaiseDerived<TConcreteEvent>(Slice slice, object? argument);
}

internal abstract class InvokersHolder<TEvent> : InvokersHolder
{
    protected InvokersHolder(bool listenAssignableEvents) : base(listenAssignableEvents) { }

    public abstract void Raise(Slice slice, TEvent argument);
}

internal sealed class InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce> : InvokersHolder<TEvent>
    where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
{
    private TCallback[]? callbacks = ArrayUtils.InitialArray<TCallback>();
    private int count;
    private int millisecondsTimestamp = Environment.TickCount;
    private bool wasCountZeroDuringLastPurge;

    public InvokersHolder(bool listenAssignableEvents) : base(listenAssignableEvents) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe(TCallback callback)
    {
        Debug.Assert(!WasPurged());
        ArrayUtils.ConcurrentAdd(ref callbacks, ref count, callback);
        if (count == 1)
            // Set the flag because we are transitioning from empty to non-empty.
            wasCountZeroDuringLastPurge = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe<TPredicator>(TPredicator predicator)
        where TPredicator : IPredicator<TCallback>
    {
        Debug.Assert(!WasPurged());
        ArrayUtils.ConcurrentRemove(ref callbacks, ref count, predicator);
    }

    public override Slice GetCallbacks()
    {
        TCallback[] callbacks_ = Utils.Take(ref callbacks);
        int count_ = count;
        if (Utils.IsToggled<TIsOnce>())
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

    public override void Clear(Slice slice)
    {
        TCallback[] callbacks_ = Utils.ExpectExactType<TCallback[]>(slice.Array);

        if (Utils.IsToggled<TIsOnce>())
        {
            ArrayUtils.ReturnArray(callbacks_, slice.Count);
#if NET7_0_OR_GREATER
            TCallbackHelper
#else
            Utils.NullRef<TCallbackHelper>()
#endif
                .Dispose(callbacks_, slice.Count);
        }
    }

    public override void Raise(Slice slice, TEvent argument)
    {
        TCallback[] callbacks_ = Utils.ExpectExactType<TCallback[]>(slice.Array);
#if NET7_0_OR_GREATER
        TCallbackHelper
#else
        Utils.NullRef<TCallbackHelper>()
#endif
            .Invoke(argument, callbacks_, slice.Count);
    }

    public override void RaiseDerived<TConcreteEvent>(Slice slice, object? argument)
    {
        Debug.Assert(typeof(TEvent).IsAssignableFrom(typeof(TConcreteEvent)));
        // In .NET all value types inherits directly from a reference type, so the parent event can never be a value type.
        Debug.Assert(!typeof(TEvent).IsValueType);
        Debug.Assert(argument is null || argument.GetType() == typeof(TConcreteEvent));

        TCallback[] callbacks_ = Utils.ExpectExactType<TCallback[]>(slice.Array);
#if NET7_0_OR_GREATER
        TCallbackHelper
#else
        Utils.NullRef<TCallbackHelper>()
#endif
            .Invoke(Utils.ExpectAssignableTypeOrNull<TEvent>(argument), callbacks_, slice.Count);
    }

    public override bool Purge(out InvokersHolderTypeKey holderType, int currentMilliseconds, int trimMilliseconds, bool hasHighMemoryPressure)
    {
        int millisecondsTimestamp_ = millisecondsTimestamp;

        if ((currentMilliseconds - millisecondsTimestamp_) <= trimMilliseconds)
            goto notRemove;

        millisecondsTimestamp = currentMilliseconds;

        TCallback[]? array = callbacks;
        Debug.Assert(array is not null);
        int count_ = count;
        {
#if NET7_0_OR_GREATER
            TCallbackHelper
#else
            Utils.NullRef<TCallbackHelper>()
#endif
                .Purge(array, ref count_);
            ArrayUtils.TryShrink(ref array, count_);
        }
        count = count_;
        callbacks = array;

        if (count == 0)
        {
            if (wasCountZeroDuringLastPurge || hasHighMemoryPressure)
                // Instance is only removed during hight memory pressure or if the instance is empty during two consecutive purges.
                goto remove;

            wasCountZeroDuringLastPurge = true;
        }

    notRemove:
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out holderType);
#else
        holderType = default;
#endif
        return false;

    remove:
        holderType = new(typeof(InvokersHolder<TEvent, TCallbackHelper, TCallback, TIsOnce>), ListenToAssignableEvents);
        return true;
    }

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
#if NET7_0_OR_GREATER
            TCallbackHelper
#else
            Utils.NullRef<TCallbackHelper>()
#endif
                .Dispose(array, count_);
            ArrayUtils.ReturnArray(array, count_);
        }
    }
}
