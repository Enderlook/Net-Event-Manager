using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal sealed partial class TypeHandle
    {
        public readonly struct StructDelegate : IDelegate<StructDelegate>
        {
            public readonly Delegate @delegate;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public StructDelegate(Delegate @delegate) => this.@delegate = @delegate;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(in StructDelegate other)
                => @delegate.Equals(other.@delegate);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Invoke<T>(T argument)
            {
                if (typeof(T) == typeof(Parameterless))
                    Unsafe.As<Action>(@delegate)();
                else
                    Unsafe.As<Action<T>>(@delegate)(argument);
            }
        }
    }
}