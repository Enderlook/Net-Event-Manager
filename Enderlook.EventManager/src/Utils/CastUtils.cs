using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        public static ref TTo ExpectExactType<TFrom, TTo>(ref TFrom obj)
        {
            Debug.Assert(obj is not null);
            Debug.Assert(obj.GetType() == typeof(TTo));
            return ref Unsafe.As<TFrom, TTo>(ref obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo ExpectAssignableType<TTo>(object obj) where TTo : class
        {
            Debug.Assert(obj is not null);
            Debug.Assert(obj is TTo);
            return Unsafe.As<TTo>(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [return: NotNullIfNotNull("value")]
        public static object? AsObject<T>(T? value)
        {
            Debug.Assert(!typeof(T).IsValueType);
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            return value;
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }
    }
}