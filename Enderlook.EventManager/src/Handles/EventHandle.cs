using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed class EventHandle<TList, TDelegate, TMode, TClosure, TEvent> : RaisableHandle<TEvent>
        where TList : IEventCollection<TDelegate>
    {
        private TList list;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventHandle()
        {
            if (typeof(TList) == typeof(EventList<TDelegate>))
            {
                EventList<TDelegate> eventList = EventList<TDelegate>.Create();
                list = Unsafe.As<EventList<TDelegate>, TList>(ref eventList);
            }
            else if (typeof(TList) == typeof(EventListOnce<TDelegate>))
            {
                EventListOnce<TDelegate> eventList = EventListOnce<TDelegate>.Create();
                list = Unsafe.As<EventListOnce<TDelegate>, TList>(ref eventList);
            }
            else
                Debug.Fail("Impossible state.");
        }

        public override void Compact() => list.Compact();

        public override void CompactAndPurge() => list.CompactAndPurge();

        public override void Dispose() => list.Dispose();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TDelegate @delegate) => list.Add(@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TDelegate @delegate) => list.Remove(@delegate);

        public override void GetSnapshoot(out Array obj, out int count)
        {
            List<TDelegate> toExecute = list.GetExecutionList();
            obj = toExecute.UnderlyingObject;
            count = toExecute.Count;
        }

        public override void Raise(Array obj, int count, TEvent argument)
        {
            List<TDelegate> toExecute = new(new(obj), count);
            for (int i = 0; i < toExecute.Count; i++)
                Execute(toExecute[i], argument);
            list.ReturnExecutionList(toExecute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Execute(TDelegate @delegate, TEvent argument)
        {
            // TODO: Add debug assertions for weak delegates.

            if (typeof(TMode) == typeof(HasNoClosureStrong))
            {
                Debug.Assert(typeof(TClosure) == typeof(Unused));
                if (typeof(TEvent) == typeof(Unused))
                {
                    Debug.Assert(@delegate is Action);
                    Unsafe.As<Action>(@delegate)();
                }
                else
                {
                    Debug.Assert(@delegate is Action<TEvent>);
                    Unsafe.As<Action<TEvent>>(@delegate)(argument);
                }
            }
            else if (typeof(TMode) == typeof(HasNoClosureWeakWithHandle))
            {
                Debug.Assert(typeof(TClosure) == typeof(Unused));
                Debug.Assert(typeof(IWeak).IsAssignableFrom(typeof(TDelegate)));

                Debug.Assert(@delegate is WeakDelegate);
                WeakDelegate weak = Unsafe.As<TDelegate, WeakDelegate>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    // TODO: This assertion should be improved.
                    Debug.Assert(!weak.@delegate.GetType().GetGenericArguments()[0].IsValueType);
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;
                    Unsafe.As<Action<object>>(weak.@delegate)(target);
                }
                else
                {
                    // TODO: This assertion should be improved.
                    Debug.Assert(!weak.@delegate.GetType().GetGenericArguments()[0].IsValueType);
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;
                    Unsafe.As<Action<object, TEvent>>(weak.@delegate)(target, argument);
                }
            }
            else if (typeof(TMode) == typeof(HasNoClosureWeakWithoutHandle))
            {
                Debug.Assert(typeof(TClosure) == typeof(Unused));
                Debug.Assert(typeof(IWeak).IsAssignableFrom(typeof(TDelegate)));

                Debug.Assert(@delegate is WeakDelegate);
                WeakDelegate weak = Unsafe.As<TDelegate, WeakDelegate>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    Debug.Assert(weak.@delegate is Action);
                    if (weak.Handle.Target is null)
                        return;
                    Unsafe.As<Action>(weak.@delegate)();
                }
                else
                {
                    Debug.Assert(weak.@delegate is Action<TEvent>);
                    if (weak.Handle.Target is null)
                        return;
                    Unsafe.As<Action<TEvent>>(weak.@delegate)(argument);
                }
            }
            else if (typeof(TMode) == typeof(HasClosureStrong))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));
                Debug.Assert(@delegate is ClosureDelegate<TClosure>);

                ClosureDelegate<TClosure> closure = Unsafe.As<TDelegate, ClosureDelegate<TClosure>>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    if (typeof(TClosure) == typeof(object))
                        Debug.Assert(closure.@delegate.GetType() == typeof(Action<>).MakeGenericType(@delegate.GetType().GetGenericArguments()[0]));
                    else
                        Debug.Assert(closure.@delegate is Action<TClosure>);

                    Unsafe.As<Action<TClosure>>(closure.@delegate)(closure.closure);
                }
                else
                {
                    if (typeof(TClosure) == typeof(object))
                        Debug.Assert(closure.@delegate.GetType() == typeof(Action<,>).MakeGenericType(@delegate.GetType().GetGenericArguments()[0], typeof(TEvent)));
                    else
                        Debug.Assert(closure.@delegate is Action<TClosure, TEvent>);

                    Unsafe.As<Action<TClosure, TEvent>>(closure.@delegate)(closure.closure, argument);
                }
            }
            else if (typeof(TMode) == typeof(HasClosureWeakWithoutHandle))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));

                Debug.Assert(@delegate is WeakDelegate<TClosure>);
                WeakDelegate<TClosure> weak = Unsafe.As<TDelegate, WeakDelegate<TClosure>>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    if (typeof(TClosure) == typeof(object))
                        Debug.Assert(weak.@delegate.GetType() == typeof(Action<>).MakeGenericType(@delegate.GetType().GetGenericArguments()[0]));
                    else
                        Debug.Assert(weak.@delegate is Action<TClosure>);

                    // TODO: This assertion should be improved.
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;

                    Unsafe.As<Action<TClosure>>(weak.@delegate)(weak.closure);
                }
                else
                {
                    if (typeof(TClosure) == typeof(object))
                        Debug.Assert(weak.@delegate.GetType() == typeof(Action<,>).MakeGenericType(@delegate.GetType().GetGenericArguments()[0], typeof(TEvent)));
                    else
                        Debug.Assert(weak.@delegate is Action<TClosure, TEvent>);

                    // TODO: This assertion should be improved.
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;

                    Unsafe.As<Action<TClosure, TEvent>>(weak.@delegate)(weak.closure, argument);
                }
            }
            else if (typeof(TMode) == typeof(HasClosureWeakWithHandle))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));

                Debug.Assert(@delegate is WeakDelegate<TClosure>);
                WeakDelegate<TClosure> weak = Unsafe.As<TDelegate, WeakDelegate<TClosure>>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    // TODO: This assertion should be improved.
                    Debug.Assert(!weak.@delegate.GetType().GetGenericArguments()[0].IsValueType);
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;
                    Unsafe.As<Action<object, TClosure>>(weak.@delegate)(target, weak.closure);
                }
                else
                {
                    // TODO: This assertion should be improved.
                    Debug.Assert(!weak.@delegate.GetType().GetGenericArguments()[0].IsValueType);
                    object target = weak.Handle.Target;
                    if (target is null)
                        return;
                    Unsafe.As<Action<object, TClosure, TEvent>>(weak.@delegate)(target, weak.closure, argument);
                }
            }
            else
                Debug.Fail("Impossible state.");
        }
    }
}