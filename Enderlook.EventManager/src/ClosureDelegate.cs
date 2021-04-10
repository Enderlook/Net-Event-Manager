using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct ClosureDelegate<TClosure, TEvent> : IDelegate<ClosureDelegate<TClosure, TEvent>, TEvent>
    {
        public readonly Action<TClosure, TEvent> @delegate;
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ClosureDelegate(Action<TClosure, TEvent> @delegate, TClosure closure)
        {
            this.@delegate = @delegate;
            this.closure = closure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in ClosureDelegate<TClosure, TEvent> other)
            => @delegate.Equals(other.@delegate) && EqualityComparer<TClosure>.Default.Equals(closure, other.closure);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument)
        {
            if (typeof(TEvent) == typeof(Parameterless))
                Unsafe.As<Action<TClosure>>(@delegate)(closure);
            else
                @delegate(closure, argument);
        }
    }
}