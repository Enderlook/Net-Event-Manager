using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal static class Utility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Execute<TDelegate, TEvent, TMode, TClosure>(TDelegate @delegate, TEvent argument)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Raise<TDelegate, TEvent, TMode, TClosure>(List<TDelegate> list, TEvent argument)
        {
            if (list.Count == 0)
                return;

            TDelegate _ = list[list.Count - 1];
            for (int i = 0; i < list.Count; i++)
                Execute<TDelegate, TEvent, TMode, TClosure>(list[i], argument);
        }
    }
}