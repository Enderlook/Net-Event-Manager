using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct ClosureDelegate<TClosure>
    {
        public readonly Delegate @delegate;
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ClosureDelegate(Delegate @delegate, TClosure closure)
        {
            this.@delegate = @delegate;
            this.closure = closure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in ClosureDelegate<TClosure> other)
            => @delegate.Equals(other.@delegate) && EqualityComparer<TClosure>.Default.Equals(closure, other.closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke<TEvent>(TEvent argument)
        {
            if (typeof(TEvent) == typeof(HasNoParameter))
            {
                Debug.Assert(@delegate is Action<TClosure>);
                Unsafe.As<Action<TClosure>>(@delegate)(closure);
            }
            else
            {
                Debug.Assert(@delegate is Action<TClosure, TEvent>);
                Unsafe.As<Action<TClosure, TEvent>>(@delegate)(closure, argument);
            }
        }
    }
}