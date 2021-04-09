using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle
    {
        public readonly struct ClosureDelegate<T> : IDelegate<ClosureDelegate<T>>
        {
            public readonly Delegate @delegate;
            public readonly T closure;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ClosureDelegate(Delegate @delegate, T closure)
            {
                this.@delegate = @delegate;
                this.closure = closure;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(in ClosureDelegate<T> other)
                => @delegate.Equals(other.@delegate) && EqualityComparer<T>.Default.Equals(closure, other.closure);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Invoke<U>(U argument)
            {
                if (typeof(U) == typeof(Parameterless))
                    Unsafe.As<Action<T>>(@delegate)(closure);
                else
                    Unsafe.As<Action<T, U>>(@delegate)(closure, argument);
            }
        }
    }
}