using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolderManager
{
    // Element types are InvokersHolder<TEvent>.
    protected InvariantObject[]? holders = ArrayUtils.InitialArray<InvariantObject>();
    protected int holdersCount;

    // Element types are InvokersHolder<T> where typeof(T).IsAssignableFrom(TEvent).
    internal InvariantObject[] derivedHolders = ArrayUtils.EmptyArray<InvariantObject>();
    internal int derivedHoldersCount;

    private int millisecondsTimestamp;
    private bool wasRaisedDuringLastPurge;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void SetAsRaised() => wasRaisedDuringLastPurge = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(InvokersHolder holder)
    {
        Debug.Assert(GetType().GenericTypeArguments[0] == holder.GetType().GenericTypeArguments[0]);
        ArrayUtils.ConcurrentAdd(ref holders, ref holdersCount, holder.Wrap());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddDerived(InvokersHolder holder, Type concreteEventType)
    {
        Debug.Assert(concreteEventType.IsAssignableFrom(GetType().GenericTypeArguments[0]));
        Debug.Assert(holder.GetType().GenericTypeArguments[0] == concreteEventType);
        // This lock is required to prevent a data invalidation in the Raise methods.
        InvariantObject[] @lock = Utils.Take(ref holders);
        {
            ArrayUtils.Add(ref derivedHolders, ref derivedHoldersCount, holder.Wrap());
        }
        Utils.Untake(ref holders, @lock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTo(InvokersHolderManager holderManager, Type concreteEventType)
    {
        Debug.Assert(holderManager.GetType().GenericTypeArguments[0] == concreteEventType);
        Debug.Assert(GetType().GenericTypeArguments[0].IsAssignableFrom(holderManager.GetType().GenericTypeArguments[0]));
        // This lock is required to prevent a data invalidation in the Raise methods.
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            InvariantObject[] @lock = Utils.Take(ref holderManager.holders);
            {
                InvariantObject[]? derivedHolders = holderManager.derivedHolders;
                int derivedHoldersCount = holderManager.derivedHoldersCount;
                {
                    for (int i = 0; i < holdersCount; i++)
                    {
                        InvariantObject holder = takenHolders[i];
                        if (Utils.ExpectAssignableType<InvokersHolder>(holder.Unwrap()).ListenToAssignableEvents)
                            ArrayUtils.Add(ref derivedHolders, ref derivedHoldersCount, holder);
                    }
                }
                holderManager.derivedHolders = derivedHolders;
                holderManager.derivedHoldersCount = derivedHoldersCount;
            }
            Utils.Untake(ref holderManager.holders, @lock);
        }
        Utils.Untake(ref holders, takenHolders);
    }

    protected abstract Type GetEventType();

    public abstract void DynamicRaise<TBaseEvent>(TBaseEvent? argument, EventManager eventManager);

    public bool Purge([NotNullWhen(true)] out Type eventType, int currentMilliseconds, int trimMilliseconds, bool hasHighMemoryPressure)
    {
        int millisecondsTimestamp_ = millisecondsTimestamp;

        if ((currentMilliseconds - millisecondsTimestamp_) <= trimMilliseconds)
            goto notRemove;

        millisecondsTimestamp = currentMilliseconds;

        Debug.Assert(holders is not null);
        int holdersCount_ = holdersCount;
        Purge(ref holders, ref holdersCount_);
        holdersCount = holdersCount_;
        Purge(ref derivedHolders, ref derivedHoldersCount);

        if (holdersCount_ == 0)
        {
            if (hasHighMemoryPressure || !wasRaisedDuringLastPurge)
                goto remove;
            wasRaisedDuringLastPurge = false;
        }

    notRemove:
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out eventType);
#else
        eventType = default;
#endif
        return false;

    remove:
        eventType = GetEventType();
        return true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Purge(ref InvariantObject[] array, ref int count)
        {
            InvariantObject[] array_ = array;
            int count_ = count;
            {
                if (count_ != 0)
                {
                    ref InvariantObject start = ref Utils.GetArrayDataReference(array_);
                    ref InvariantObject current = ref start;
                    ref InvariantObject end = ref Unsafe.Add(ref current, count_);
                    while (Unsafe.IsAddressLessThan(ref current, ref end))
                    {
                        if (Utils.ExpectAssignableType<InvokersHolder>(current.Unwrap()).WasPurged())
                        {
                            end = ref Unsafe.Subtract(ref end, 1);
                            current = end;
                            end = default!;
#if DEBUG
                            count_--;
#endif
                        }
                        current = ref Unsafe.Add(ref current, 1);
                    }
                    unsafe
                    {
                        count = (int)((new IntPtr(Unsafe.AsPointer(ref end)).ToInt64() - new IntPtr(Unsafe.AsPointer(ref start)).ToInt64()) / Unsafe.SizeOf<InvariantObject>());
#if DEBUG
                        Debug.Assert(count == count_);
#endif
                    }
                }
                ArrayUtils.TryShrink(ref array_, count);
            }
            array = array_;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Debug.Assert(holders is not null);
        ArrayUtils.ReturnArray(holders, holdersCount);
        ArrayUtils.ReturnArray(derivedHolders, derivedHoldersCount);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static SliceOfCallbacks[]? GetSlices(InvariantObject[] holders, int count)
    {
        SliceOfCallbacks[]? slices;
        if (count > 0)
        {
            slices = ArrayUtils.RentArray<SliceOfCallbacks>(count);
            ref InvariantObject currentHolder = ref Utils.GetArrayDataReference(holders);
            ref InvariantObject endHolder = ref Unsafe.Add(ref currentHolder, count);
            ref SliceOfCallbacks sliceCurrent = ref Utils.GetArrayDataReference(slices);

            while (Unsafe.IsAddressLessThan(ref currentHolder, ref endHolder))
            {
                InvokersHolder? holder = Utils.ExpectAssignableType<InvokersHolder>(currentHolder.Unwrap());
                Slice callbacks = holder.GetCallbacks();
                sliceCurrent = new(holder, callbacks);

                currentHolder = ref Unsafe.Add(ref currentHolder, 1);
                sliceCurrent = ref Unsafe.Add(ref sliceCurrent, 1);
            }
        }
        else
            slices = null;
        return slices;
    }
}

internal sealed class InvokersHolderManager<TEvent> : InvokersHolderManager
{
    protected override Type GetEventType() => typeof(TEvent);

    public override void DynamicRaise<TBaseEvent>(TBaseEvent? argument, EventManager eventManager)
        where TBaseEvent : default
    {
        Debug.Assert(typeof(TEvent) == (argument?.GetType() ?? typeof(TBaseEvent)));

        SetAsRaised();

        int holdersCount_;
        int derivedHoldersCount_;
        SliceOfCallbacks[]? derivedSlicers;
        SliceOfCallbacks[]? slices;
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            holdersCount_ = holdersCount;
            derivedHoldersCount_ = derivedHoldersCount;

            derivedSlicers = GetSlices(derivedHolders, derivedHoldersCount_);
            slices = GetSlices(takenHolders, holdersCount_);
        }
        Utils.Untake(ref holders, takenHolders);

        eventManager.InHolderEnd();

        try
        {
            if (holdersCount_ > 0)
            {
                TEvent? argument_;
                if (typeof(TEvent).IsValueType)
                {
                    Debug.Assert(argument is not null);
                    argument_ = (TEvent)(object)argument;
                }
                else
                    argument_ = Utils.ExpectExactTypeOrNull<TEvent>(argument);

                RaiseSlice(argument_, slices, holdersCount_);
            }

            if (derivedHoldersCount_ > 0)
            {
                // If typeof(TEvent).IsValueType, then derived are reference types since in .NET all value types inherits directly from a reference type,
                // so we may need to box the argument.
                object? boxedArgument = argument;
                RaiseDerivedSlice(boxedArgument, derivedSlicers, derivedHoldersCount_);
            }
        }
        finally
        {
            if (holdersCount_ > 0)
                Clear(slices, holdersCount_);

            if (derivedHoldersCount_ > 0)
            {
                Debug.Assert(derivedSlicers is not null);
                Clear(derivedSlicers, derivedHoldersCount_);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StaticRaise(TEvent? argument, EventManager eventManager)
    {
        SetAsRaised();

        int holdersCount_;
        int derivedHoldersCount_;
        SliceOfCallbacks[]? derivedSlicers;
        SliceOfCallbacks[]? slices;
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            holdersCount_ = holdersCount;
            derivedHoldersCount_ = derivedHoldersCount;

            derivedSlicers = GetSlices(derivedHolders, derivedHoldersCount_);
            slices = GetSlices(takenHolders, holdersCount_);
        }
        Utils.Untake(ref holders, takenHolders);

        eventManager.InHolderEnd();

        try
        {
            if (holdersCount_ > 0)
                RaiseSlice(argument, slices, holdersCount_);

            if (derivedHoldersCount_ > 0)
            {
                Debug.Assert(derivedSlicers is not null);
                // If typeof(TEvent).IsValueType, then derived are reference types since in .NET all value types inherits directly from a reference type,
                // so we may need to box the argument.
                object? boxedArgument = argument;
                RaiseDerivedSlice(boxedArgument, derivedSlicers, derivedHoldersCount_);
            }
        }
        finally
        {
            if (holdersCount_ > 0)
                Clear(slices, holdersCount_);

            if (derivedHoldersCount_ > 0)
            {
                Debug.Assert(derivedSlicers is not null);
                Clear(derivedSlicers, derivedHoldersCount_);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Clear(SliceOfCallbacks[]? slices, int count)
    {
        Debug.Assert(count > 0);

        Debug.Assert(slices is not null);
        ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
        ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, count);

        while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
        {
            currentSlice.Clear();
            currentSlice = ref Unsafe.Add(ref currentSlice, 1);
        }
        ArrayUtils.ReturnArray(slices, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RaiseSlice(TEvent? argument, SliceOfCallbacks[]? slices, int count)
    {
        Debug.Assert(count > 0);

        Debug.Assert(slices is not null);
        ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
        ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, count);

        while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
        {
            currentSlice.Raise(argument);
            currentSlice = ref Unsafe.Add(ref currentSlice, 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void RaiseDerivedSlice(object? argument, SliceOfCallbacks[]? slices, int count)
    {
        Debug.Assert(count > 0);

        Debug.Assert(slices is not null);
        ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
        ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, count);

        while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
        {
            currentSlice.RaiseDerived<TEvent>(argument);
            currentSlice = ref Unsafe.Add(ref currentSlice, 1);
        }
    }
}