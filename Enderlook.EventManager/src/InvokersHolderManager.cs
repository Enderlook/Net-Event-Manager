using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal abstract class InvokersHolderManager
{
    public abstract void Remove(InvokersHolder holder);
}

internal class InvokersHolderManager<TEvent> : InvokersHolderManager
{
    private object[]? holders = ArrayUtils.InitialArray<object>();
    private int count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(InvokersHolder<TEvent> holder)
        => ArrayUtils.ConcurrentAdd(ref holders, ref count, holder);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Raise(TEvent? argument, EventManager eventManager)
    {
        object[] holders_ = Utils.Take(ref holders);
        {
            int count_ = count;

            if (count_ == 0)
            {
                Utils.Untake(ref holders, holders_);
                return;
            }

            SliceOfCallbacks[] slices = ArrayUtils.RentArray<SliceOfCallbacks>(count_);
            {
                ref object currentHolder = ref Utils.GetArrayDataReference(holders_);
                ref object endHolder = ref Unsafe.Add(ref currentHolder, count_);
                ref SliceOfCallbacks sliceCurrent = ref Utils.GetArrayDataReference(slices);

                while (Unsafe.IsAddressLessThan(ref currentHolder, ref endHolder))
                {
                    InvokersHolder<TEvent>? holder = Utils.ExpectAssignableType<InvokersHolder<TEvent>>(currentHolder);
                    Slice callbacks = holder.GetCallbacks();
                    sliceCurrent = new(holder, callbacks);

                    currentHolder = ref Unsafe.Add(ref currentHolder, 1);
                    sliceCurrent = ref Unsafe.Add(ref sliceCurrent, 1);
                }

                Utils.Untake(ref holders, holders_);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Remove(InvokersHolder holder)
    {
        object[]? holders_ = holders;
        Debug.Assert(holders_ is not null);
        int count_ = count;
        ref object start = ref Utils.GetArrayDataReference(holders_);
        ref object current = ref start;
        ref object end = ref Unsafe.Add(ref current, count_);
        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            if (ReferenceEquals(current, holder))
            {
                current = Unsafe.Add(ref start, count_ - 1);
                Unsafe.Add(ref start, count - 1) = null!;
            }
            current = ref Unsafe.Add(ref current, 1);
        }
    }
}
