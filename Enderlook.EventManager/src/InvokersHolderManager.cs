using System;
using System.Diagnostics;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(InvokersHolder holder)
    {
        Debug.Assert(GetType().GenericTypeArguments[0] == holder.GetType().GenericTypeArguments[0]);
        ArrayUtils.ConcurrentAdd(ref holders, ref holdersCount, new(holder));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddDerived(InvokersHolder holder, Type concreteEventType)
    {
        Debug.Assert(concreteEventType.IsAssignableFrom(GetType().GenericTypeArguments[0]));
        Debug.Assert(holder.GetType().GenericTypeArguments[0] == concreteEventType);
        // This lock is required to prevent a data invalidation in the Raise methods.
        InvariantObject[] @lock = Utils.Take(ref holders);
        {
            ArrayUtils.Add(ref derivedHolders, ref derivedHoldersCount, new(holder));
        }
        Utils.Untake(ref holders, @lock);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTo(InvokersHolderManager holderManager, Type concreteEventType)
    {
        Debug.Assert(holderManager.GetType().GenericTypeArguments[0] == concreteEventType);
        Debug.Assert(GetType().GenericTypeArguments[0].IsAssignableFrom(holderManager.GetType().GenericTypeArguments[0]));
        // This lock is required to prevent a data invalidation in the Raise methods.
        InvariantObject[] holders_ = Utils.Take(ref holders);
        {
            InvariantObject[] @lock = Utils.Take(ref holderManager.holders);
            {
                ArrayUtils.AddRange(ref holderManager.derivedHolders, ref holderManager.derivedHoldersCount, holders_, holdersCount);
            }
            Utils.Untake(ref holderManager.holders, @lock);
        }
        Utils.Untake(ref holders, holders_);
    }

    public abstract void DynamicRaiseExactly<TBaseEvent>(TBaseEvent? argument, EventManager eventManager);

    public abstract void DynamicRaiseHierarchy<TBaseEvent>(TBaseEvent? argument, EventManager eventManager);

    public void Remove(InvokersHolder holder)
    {
        InvariantObject[]? holders_ = holders;
        Debug.Assert(holders_ is not null);
        int holdersCount_ = holdersCount;
        ref InvariantObject start = ref Utils.GetArrayDataReference(holders_);
        ref InvariantObject current = ref start;
        ref InvariantObject end = ref Unsafe.Add(ref current, holdersCount_);
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (ReferenceEquals(current.Value, holder))
            {
                end = ref Unsafe.Subtract(ref end, 1);
                current = end;
                end = new(null!);
                holdersCount = holdersCount_ - 1;
                break;
            }
            current = ref Unsafe.Add(ref current, 1);
        }
    }

    public void RemoveRemovedDerived()
    {
        InvariantObject[]? derivedHolders_ = derivedHolders;
        Debug.Assert(derivedHolders_ is not null);
        int derivedHoldersCount_ = derivedHoldersCount;
        ref InvariantObject start = ref Utils.GetArrayDataReference(derivedHolders_);
        ref InvariantObject current = ref start;
        ref InvariantObject end = ref Unsafe.Add(ref current, derivedHoldersCount_);
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (Utils.ExpectAssignableType<InvokersHolder>(current.Value).WasRemoved())
            {
                end = ref Unsafe.Subtract(ref end, 1);
                current = end;
                end = new(null!);
#if DEBUG
                derivedHoldersCount_--;
#endif
            }
            current = ref Unsafe.Add(ref current, 1);
        }
        unsafe
        {
            derivedHoldersCount = (int)((new IntPtr(Unsafe.AsPointer(ref end)).ToInt64() - new IntPtr(Unsafe.AsPointer(ref start)).ToInt64()) / Unsafe.SizeOf<InvariantObject>());
#if DEBUG
            Debug.Assert(derivedHoldersCount == derivedHoldersCount_);
#endif
        }
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
                InvokersHolder? holder = Utils.ExpectAssignableType<InvokersHolder>(currentHolder.Value);
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
    public override void DynamicRaiseExactly<TBaseEvent>(TBaseEvent? argument, EventManager eventManager)
        where TBaseEvent : default
    {
        Debug.Assert(typeof(TEvent) == (argument?.GetType() ?? typeof(TBaseEvent)));

        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int holdersCount_ = holdersCount;

            SliceOfCallbacks[]? slices = GetSlices(takenHolders, holdersCount_);
            Utils.Untake(ref holders, takenHolders);

            eventManager.InHolderEnd();

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
        }
    }

    public override void DynamicRaiseHierarchy<TBaseEvent>(TBaseEvent? argument, EventManager eventManager)
        where TBaseEvent : default
    {
        Debug.Assert(typeof(TEvent) == (argument?.GetType() ?? typeof(TBaseEvent)));

        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int holdersCount_ = holdersCount;
            int derivedHoldersCount_ = derivedHoldersCount;

            SliceOfCallbacks[]? derivedSlicers = GetSlices(derivedHolders, derivedHoldersCount_);
            SliceOfCallbacks[]? slices = GetSlices(takenHolders, holdersCount_);
            Utils.Untake(ref holders, takenHolders);

            eventManager.InHolderEnd();

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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StaticRaiseExactly(TEvent? argument, EventManager eventManager)
    {
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int holdersCount_ = holdersCount;

            SliceOfCallbacks[]? slices = GetSlices(takenHolders, holdersCount_);
            Utils.Untake(ref holders, takenHolders);

            eventManager.InHolderEnd();

            if (holdersCount_ > 0)
                RaiseSlice(argument, slices, holdersCount_);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StaticRaiseHierarchy(TEvent? argument, EventManager eventManager)
    {
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int holdersCount_ = holdersCount;
            int derivedHoldersCount_ = derivedHoldersCount;

            SliceOfCallbacks[]? derivedSlicers = GetSlices(derivedHolders, derivedHoldersCount_);
            SliceOfCallbacks[]? slices = GetSlices(takenHolders, holdersCount_);
            Utils.Untake(ref holders, takenHolders);

            eventManager.InHolderEnd();

            if (holdersCount_ > 0)
                RaiseSlice(argument, slices, holdersCount_);

            if (derivedHoldersCount_ > 0)
            {
                Debug.Assert(derivedSlicers is not null);
                ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(derivedSlicers);
                ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, derivedHoldersCount_);

                // If typeof(TEvent).IsValueType, then derived are reference types since in .NET all value types inherits directly from a reference type,
                // so we may need to box the argument.
                object? boxedArgument = argument;
                RaiseDerivedSlice(boxedArgument, derivedSlicers, derivedHoldersCount_);
            }
        }
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
        ArrayUtils.ReturnArray(slices, count);
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
        ArrayUtils.ReturnArray(slices, count);
    }
}