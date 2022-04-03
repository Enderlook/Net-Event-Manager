using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Enderlook.EventManager;

internal static class Utils
{
    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ExpectExactType<TTo>(object obj)
    {
        Debug.Assert(obj is not null);
        Debug.Assert(!typeof(TTo).IsValueType);
        Debug.Assert(obj!.GetType() == typeof(TTo));
        return Unsafe.As<object, TTo>(ref obj)!;
    }

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ExpectAssignableType<TTo>(object obj)
    {
        Debug.Assert(obj is not null);
        Debug.Assert(!typeof(TTo).IsValueType);
        Debug.Assert(typeof(TTo).IsAssignableFrom(obj!.GetType()));
        return Unsafe.As<object, TTo>(ref obj)!;
    }

    [return: NotNullIfNotNull("obj")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ExpectExactTypeOrNull<TTo>(object? obj)
    {
        Debug.Assert(!typeof(TTo).IsValueType);
        Debug.Assert(obj is null || obj.GetType() == typeof(TTo));
        return Unsafe.As<object?, TTo>(ref obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike<T1>(object obj, T1 t1)
    {
#if DEBUG
        Debug.Assert(obj is not null);
        Type type = obj!.GetType();
        Debug.Assert(type.GetGenericTypeDefinition() == typeof(Action<>));
        Type[] types = type.GetGenericArguments();
        Debug.Assert(types[0] == typeof(T1));
#endif
        Unsafe.As<Action<T1>>(obj)(t1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike<T1, T2>(object obj, T1 t1, T2 t2)
    {
#if DEBUG
        Debug.Assert(obj is not null);
        Type type = obj!.GetType();
        Debug.Assert(type.GetGenericTypeDefinition() == typeof(Action<,>));
        Type[] types = type.GetGenericArguments();
        Debug.Assert(types[0] == typeof(T1));
        Debug.Assert(types[1] == typeof(T2));
#endif
        Unsafe.As<Action<T1, T2>>(obj)(t1, t2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike<T1, T2, T3>(object obj, T1 t1, T2 t2, T3 t3)
    {
#if DEBUG
        Debug.Assert(obj is not null);
        Type type = obj!.GetType();
        Debug.Assert(type.GetGenericTypeDefinition() == typeof(Action<,,>));
        Type[] types = type.GetGenericArguments();
        Debug.Assert(types[0] == typeof(T1));
        Debug.Assert(types[1] == typeof(T2));
        Debug.Assert(types[2] == typeof(T3));
#endif
        Unsafe.As<Action<T1, T2, T3>>(obj)(t1, t2, t3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike(object obj) => ExpectExactType<Action>(obj)();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1>(object? handle, object? obj, T1 t1)
    {
        if (handle is null)
            return;
        Debug.Assert(obj is not null);
        ExecuteActionLike(obj, t1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1, T2>(object? handle, object? obj, T1 t1, T2 t2)
    {
        if (handle is null)
            return;
        Debug.Assert(obj is not null);
        ExecuteActionLike(obj, t1, t2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1, T2, T3>(object? handle, object? obj, T1 t1, T2 t2, T3 t3)
    {
        if (handle is null)
            return;
        Debug.Assert(obj is not null);
        ExecuteActionLike(obj, t1, t2, t3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike(object? handle, object? obj)
    {
        if (handle is null)
            return;
        Debug.Assert(obj is not null);
        ExecuteActionLike(obj);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Take<T>(ref T? obj) where T : class
    {
        T? obj_;
        do
        {
            obj_ = Interlocked.Exchange(ref obj, null);
        } while (obj_ is null);
        return obj_;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Untake<T>(ref T? obj, T obj_)
    {
        Debug.Assert(obj is null);
        Debug.Assert(obj_ is not null);
        obj = obj_;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetArrayDataReference<T>(T[] array)
    {
        Debug.Assert(array.Length > 0);
#if NET5_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(array);
#elif NETSTANDARD2_1_OR_GREATER
        return ref MemoryMarshal.GetReference((Span<T>)array);
#else
        return ref array[0];
#endif
    }

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Null<T>() where T : struct => ref Unsafe.NullRef<T>();
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Null<T>() where T : struct => new();
#endif
}
