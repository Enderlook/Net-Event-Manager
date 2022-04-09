using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Enderlook.EventManager;

internal static class Utils
{
    public static MemoryPressure GetMemoryPressure()
    {
#if NET5_0_OR_GREATER
        const double HighPressureThreshold = .90; // Percent of GC memory pressure threshold we consider "high".
        const double MediumPressureThreshold = .70; // Percent of GC memory pressure threshold we consider "medium".

        GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();

        if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * HighPressureThreshold)
            return MemoryPressure.High;

        if (memoryInfo.MemoryLoadBytes >= memoryInfo.HighMemoryLoadThresholdBytes * MediumPressureThreshold)
            return MemoryPressure.Medium;

        return MemoryPressure.Low;
#else
        return MemoryPressure.High;
#endif
    }

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

    [return: NotNull]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TTo ExpectAssignableTypeOrNull<TTo>(object? obj)
    {
        Debug.Assert(!typeof(TTo).IsValueType);
        Debug.Assert(obj is null || typeof(TTo).IsAssignableFrom(obj.GetType()));
        return Unsafe.As<object?, TTo>(ref obj)!;
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
    public static void ExecuteActionLike<T1>(object action, T1 t1)
    {
        Debug.Assert(action.GetType() == typeof(Action<>).MakeGenericType(typeof(T1)));
        Unsafe.As<Action<T1>>(action)(t1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike<T1, T2>(object action, T1 t1, T2 t2)
    {
        Debug.Assert(action.GetType() == typeof(Action<,>).MakeGenericType(typeof(T1), typeof(T2)));
        Unsafe.As<Action<T1, T2>>(action)(t1, t2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike<T1, T2, T3>(object action, T1 t1, T2 t2, T3 t3)
    {
        Debug.Assert(action.GetType() == typeof(Action<,,>).MakeGenericType(typeof(T1), typeof(T2), typeof(T3)));
        Unsafe.As<Action<T1, T2, T3>>(action)(t1, t2, t3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExecuteActionLike(object action) => ExpectExactType<Action>(action)();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1>(object? handle, object? action, T1 t1)
    {
        if (handle is null)
            return;
        Debug.Assert(action is not null);
        ExecuteActionLike(action, t1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1, T2>(object? handle, object? action, T1 t1, T2 t2)
    {
        if (handle is null)
            return;
        Debug.Assert(action is not null);
        ExecuteActionLike(action, t1, t2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike<T1, T2, T3>(object? handle, object? action, T1 t1, T2 t2, T3 t3)
    {
        if (handle is null)
            return;
        Debug.Assert(action is not null);
        ExecuteActionLike(action, t1, t2, t3);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakExecuteActionLike(object? handle, object? action)
    {
        if (handle is null)
            return;
        Debug.Assert(action is not null);
        ExecuteActionLike(action);
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

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ref T NullRef<T>() => ref Unsafe.NullRef<T>();
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe static ref T NullRef<T>() => ref Unsafe.AsRef<T>(null);
#endif

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref T value) => Unsafe.IsNullRef(ref value);
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(ref T value) => Unsafe.AreSame(ref Utils.NullRef<T>(), ref value);
#endif
}
