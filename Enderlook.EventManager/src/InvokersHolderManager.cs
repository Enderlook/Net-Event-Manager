using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolderManager
{
    // Element types are InvokersHolder<TEvent>.
    protected InvariantObject[]? holders = ArrayUtils.InitialArray<InvariantObject>();
    protected int holdersCount;

    // Element types are InvokersHolder<T> where typeof(T).IsAssignableFrom(TEvent)
    internal InvariantObject[]? derivedHolders = ArrayUtils.EmptyArray<InvariantObject>();
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
        Debug.Assert(GetType().GenericTypeArguments[0].IsAssignableFrom(concreteEventType));
        Debug.Assert(holder.GetType().GenericTypeArguments[0] == concreteEventType);
        ArrayUtils.ConcurrentAdd(ref derivedHolders, ref derivedHoldersCount, new(holder));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddTo(InvokersHolderManager holderManager, Type concreteEventType)
    {
        Debug.Assert(holderManager.GetType().GenericTypeArguments[0] == concreteEventType);
        Debug.Assert(GetType().GenericTypeArguments[0].IsAssignableFrom(holderManager.GetType().GenericTypeArguments[0]));
        ArrayUtils.ConcurrentAddRange(ref holderManager.derivedHolders, ref holderManager.derivedHoldersCount, holders, holdersCount);
    }

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
}

internal class InvokersHolderManager<TEvent> : InvokersHolderManager
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RaiseHierarchy(TEvent? argument, EventManager eventManager)
    {
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        InvariantObject[] takenDerivedHolders = Utils.Take(ref derivedHolders);
        {
            int holdersCount_ = holdersCount;
            int derivedHoldersCount_ = derivedHoldersCount;

            SliceOfCallbacks[]? slices;
            if (holdersCount_ > 0)
            {
                slices = ArrayUtils.RentArray<SliceOfCallbacks>(holdersCount_);
                ref InvariantObject currentHolder = ref Utils.GetArrayDataReference(takenHolders);
                ref InvariantObject endHolder = ref Unsafe.Add(ref currentHolder, holdersCount_);
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
            Utils.Untake(ref holders, takenHolders);

            SliceOfCallbacks[]? derivedSlicers;
            if (derivedHoldersCount_ > 0)
            {
                derivedSlicers = ArrayUtils.RentArray<SliceOfCallbacks>(derivedHoldersCount_);
                ref InvariantObject currentHolder = ref Utils.GetArrayDataReference(takenDerivedHolders);
                ref InvariantObject endHolder = ref Unsafe.Add(ref currentHolder, holdersCount_);
                ref SliceOfCallbacks sliceCurrent = ref Utils.GetArrayDataReference(derivedSlicers);

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
                derivedSlicers = null;
            Utils.Untake(ref derivedHolders, takenDerivedHolders);

            eventManager.InHolderEnd();

            if (holdersCount_ > 0)
            {
                Debug.Assert(slices is not null);
                ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
                ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, holdersCount_);

                while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
                {
                    currentSlice.Raise(argument);
                    currentSlice = ref Unsafe.Add(ref currentSlice, 1);
                }
                ArrayUtils.ReturnArray(slices, holdersCount_);
            }

            if (derivedHoldersCount_ > 0)
            {
                Debug.Assert(derivedSlicers is not null);
                ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(derivedSlicers);
                ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, derivedHoldersCount_);

                // If typeof(TEvent).IsValueType, then derived are reference types since in .NET all value types inherits directly from a reference type,
                // so we may need to box the argument.
                object? boxedArgument = argument;

                while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
                {
                    currentSlice.RaiseDerived<TEvent>(boxedArgument);
                    currentSlice = ref Unsafe.Add(ref currentSlice, 1);
                }
                ArrayUtils.ReturnArray(derivedSlicers, derivedHoldersCount_);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RaiseExactly(TEvent? argument, EventManager eventManager)
    {
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int holdersCount_ = holdersCount;

            SliceOfCallbacks[]? slices;
            if (holdersCount_ > 0)
            {
                slices = ArrayUtils.RentArray<SliceOfCallbacks>(holdersCount_);
                ref InvariantObject currentHolder = ref Utils.GetArrayDataReference(takenHolders);
                ref InvariantObject endHolder = ref Unsafe.Add(ref currentHolder, holdersCount_);
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
            Utils.Untake(ref holders, takenHolders);

            eventManager.InHolderEnd();

            if (holdersCount_ > 0)
            {
                Debug.Assert(slices is not null);
                ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
                ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, holdersCount_);

                while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
                {
                    currentSlice.Raise(argument);
                    currentSlice = ref Unsafe.Add(ref currentSlice, 1);
                }
                ArrayUtils.ReturnArray(slices, holdersCount_);
            }
        }
    }
}
