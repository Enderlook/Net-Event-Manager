using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class MultipleStrongTypedEventHandle<TEvent, TElement> : StrongTypedEventHandle<TEvent, TElement>
    {
        public sealed override SliceWithEventHandle ConcurrentGetRaiser() =>
            // TODO: stored copies could be reduced by borrowing the array.
            new(list.ConcurrentClone().ToSlice(), this);
    }

    internal sealed class MultipleStrongWithArgumentEventHandle<TEvent> : MultipleStrongTypedEventHandle<TEvent, EquatableDelegate>
    {
        // We use EquatableDelegate instead of object to prevent covariant checks during array access.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TEvent> callback) => base.Add(new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TEvent> callback) => base.Remove(new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_Event(slice, argument);
    }

    internal sealed class MultipleStrongEventHandle<TEvent> : MultipleStrongTypedEventHandle<TEvent, EquatableDelegate>
    {
        // We use EquatableDelegate instead of object to prevent covariant checks during array access.

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action callback) => base.Add(new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action callback) => base.Remove(new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_(slice);
    }

    internal sealed class MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> : MultipleStrongTypedEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TClosure, TEvent> callback, TClosure closure) => Add(new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TClosure, TEvent> callback, TClosure closure) => Remove(new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_ClosureEvent<TClosure, TEvent>(slice, argument);
    }

    internal sealed class MultipleStrongWithClosureEventHandle<TEvent, TClosure> : MultipleStrongTypedEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TClosure> callback, TClosure closure) => Add(new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TClosure> callback, TClosure closure) => Remove(new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_Closure<TClosure>(slice);
    }
}