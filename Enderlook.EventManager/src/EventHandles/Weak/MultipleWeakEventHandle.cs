using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal abstract class MultipleWeakEventHandle<TEvent, TElement> : WeakTypedEventHandle<TEvent, TElement>
        where TElement : IEquatable<TElement>
    {
        public sealed override SliceWithEventHandle ConcurrentGetRaiser()
        {
            // TODO: stored copies could be reduced by borrowing the array.
            ValueList<WeakDelegate<TElement>> list_ = list.Lock();
            WeakDelegate<TElement>.FreeExpired(ref list_);
            ValueList<WeakDelegate<TElement>> clone = list_.Clone();
            list.Unlock(list_);
            return new(clone.ToSlice(), this);
        }
    }

    internal sealed class MultipleWeakWithArgumentEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<TEvent> callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_Event(slice, argument);
    }

    internal sealed class MultipleWeakEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_(slice);
    }

    internal sealed class MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_ClosureEvent<TClosure, TEvent>(slice, argument);
    }

    internal sealed class MultipleWeakWithClosureEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_Closure<TClosure>(slice);
    }

    internal sealed class MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_HandleClosureEvent<TClosure, TEvent>(slice, argument);
    }

    internal sealed class MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_ClosureHandle<TClosure>(slice);
    }

    internal sealed class MultipleWeakWithArgumentWithHandleEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_HandleEvent(slice, argument);
    }

    internal sealed class MultipleWeakWithHandleEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => WeakTypedEventHandleHelper.ConcurrentRaise_Handle(slice);
    }
}