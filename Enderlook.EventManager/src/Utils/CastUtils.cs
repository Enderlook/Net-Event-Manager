using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal static class CastUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo ExpectExactType<TTo>(object obj) where TTo : class
        {
            Debug.Assert(obj is not null);
            Debug.Assert(obj.GetType() == typeof(TTo));
            return Unsafe.As<TTo>(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo ExpectAssignableType<TTo>(object obj) where TTo : class
        {
            Debug.Assert(obj is not null);
            Debug.Assert(obj is TTo);
            return Unsafe.As<TTo>(obj);
        }
    }
}