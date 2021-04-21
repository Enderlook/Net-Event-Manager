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
            if (typeof(TMode) == typeof(HasNoClosure))
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
            else if (typeof(TMode) == typeof(HasClosure))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));

                Debug.Assert(@delegate is ClosureDelegate<TClosure>);
                ClosureDelegate<TClosure> closure = Unsafe.As<TDelegate, ClosureDelegate<TClosure>>(ref @delegate);

                if (typeof(TEvent) == typeof(Unused))
                {
                    Debug.Assert(closure.@delegate is Action<TClosure>);
                    Unsafe.As<Action<TClosure>>(closure.@delegate)(closure.closure);
                }
                else
                {
                    Debug.Assert(closure.@delegate is Action<TClosure, TEvent>);
                    Unsafe.As<Action<TClosure, TEvent>>(closure.@delegate)(closure.closure, argument);
                }
            }
            else
                Debug.Fail("Impossible state.");
        }
    }
}