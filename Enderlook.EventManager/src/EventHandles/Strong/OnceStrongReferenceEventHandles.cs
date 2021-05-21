using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class OnceStrongTypedEventHandle<TEvent, TElement> : StrongTypedEventHandle<TEvent, TElement>
    {
        public sealed override SliceWithEventHandle ConcurrentGetRaiser()
        {
            ValueList<TElement> list_ = list.Lock();
            Slice slice = list_.ToSlice();
            list.Unlock(ValueList<TElement>.Create());
            return new(slice, this);
        }
    }

    internal sealed class OnceStrongWithArgumentEventHandle<TEvent> : OnceStrongTypedEventHandle<TEvent, object>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TEvent> callback) => base.Add(callback);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TEvent> callback) => base.Remove(callback);

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_Event(slice, argument);
    }

    internal sealed class OnceStrongEventHandle<TEvent> : OnceStrongTypedEventHandle<TEvent, object>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action callback) => base.Add(callback);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action callback) => base.Remove(callback);

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_(slice);
    }

    internal sealed class OnceStrongWithArgumentWithClosureEventHandle<TEvent, TClosure> : OnceStrongTypedEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TClosure, TEvent> callback, TClosure closure) => Add(new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TClosure, TEvent> callback, TClosure closure) => Remove(new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_ClosureEvent<TClosure, TEvent>(slice, argument);
    }

    internal sealed class OnceStrongWithClosureEventHandle<TEvent, TClosure> : OnceStrongTypedEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Action<TClosure> callback, TClosure closure) => Add(new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(Action<TClosure> callback, TClosure closure) => Remove(new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument) => StrongTypedEventHandleHelper.ConcurrentRaise_Closure<TClosure>(slice);
    }
}