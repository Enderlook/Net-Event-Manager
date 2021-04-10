using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal readonly struct SimpleDelegate<TEvent> : IDelegate<SimpleDelegate<TEvent>, TEvent>
    {
        public readonly Action<TEvent> @delegate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SimpleDelegate(Action<TEvent> @delegate) => this.@delegate = @delegate;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in SimpleDelegate<TEvent> other)
            => @delegate.Equals(other.@delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument)
        {
            if (typeof(TEvent) == typeof(Parameterless))
                Unsafe.As<Action>(@delegate)();
            else
                Unsafe.As<Action<TEvent>>(@delegate)(argument);
        }
    }
}