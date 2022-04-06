using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolderManager
{
    public abstract void Remove(InvokersHolder holder);
}

internal class InvokersHolderManager<TEvent> : InvokersHolderManager
{
    private InvariantObject[]? holders = ArrayUtils.InitialArray<InvariantObject>();
    private int count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(InvokersHolder<TEvent> holder)
        => ArrayUtils.ConcurrentAdd(ref holders, ref count, new(holder));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Raise(TEvent? argument, EventManager eventManager)
    {
        InvariantObject[] takenHolders = Utils.Take(ref holders);
        {
            int count_ = count;

            if (count_ == 0)
            {
                Utils.Untake(ref holders, takenHolders);
                return;
            }

            SliceOfCallbacks[] slices = ArrayUtils.RentArray<SliceOfCallbacks>(count_);
            {
                ref InvariantObject currentHolder = ref Utils.GetArrayDataReference(takenHolders);
                ref InvariantObject endHolder = ref Unsafe.Add(ref currentHolder, count_);
                ref SliceOfCallbacks sliceCurrent = ref Utils.GetArrayDataReference(slices);

                while (Unsafe.IsAddressLessThan(ref currentHolder, ref endHolder))
                {
                    InvokersHolder<TEvent>? holder = Utils.ExpectAssignableType<InvokersHolder<TEvent>>(currentHolder.Value);
                    Slice callbacks = holder.GetCallbacks();
                    sliceCurrent = new(holder, callbacks);

                    currentHolder = ref Unsafe.Add(ref currentHolder, 1);
                    sliceCurrent = ref Unsafe.Add(ref sliceCurrent, 1);
                }

                Utils.Untake(ref holders, takenHolders);
                eventManager.InHolderEnd();

                ref SliceOfCallbacks currentSlice = ref Utils.GetArrayDataReference(slices);
                ref SliceOfCallbacks endSlice = ref Unsafe.Add(ref currentSlice, count_);

                while (Unsafe.IsAddressLessThan(ref currentSlice, ref endSlice))
                {
                    currentSlice.Raise(argument);
                    currentSlice = ref Unsafe.Add(ref currentSlice, 1);
                }
            }
            ArrayUtils.ReturnArray(slices, count_);
        }
    }

    public override void Remove(InvokersHolder holder)
    {
        InvariantObject[]? holders_ = holders;
        Debug.Assert(holders_ is not null);
        int count_ = count;
        ref InvariantObject start = ref Utils.GetArrayDataReference(holders_);
        ref InvariantObject current = ref start;
        ref InvariantObject end = ref Unsafe.Add(ref current, count_);
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (ReferenceEquals(current.Value, holder))
            {
                current = Unsafe.Add(ref start, count_ - 1);
                Unsafe.Add(ref start, count - 1) = new(null!);
            }
            current = ref Unsafe.Add(ref current, 1);
        }
    }
}
