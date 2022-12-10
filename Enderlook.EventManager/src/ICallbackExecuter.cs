﻿using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal interface ICallbackExecuter<TEvent, TCallback>
{
#if NET7_0_OR_GREATER
    static abstract
#endif
    void Invoke(TEvent argument, TCallback[] callbacks, int count);

#if NET7_0_OR_GREATER
    static abstract
#endif
    void Purge(TCallback[] callbacks, ref int count);

#if NET7_0_OR_GREATER
    static abstract
#endif
    void Dispose(TCallback[] callbacks, int count);
}

internal interface IWeak
{
    void Free();

    bool FreeIfIsCollected();
}

internal struct StrongCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallbackExecuter : ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, TCallbackExecuter>(argument, callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Purge(TCallback[] callbacks, ref int count) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Dispose(TCallback[] callbacks, int count) { }
}

internal struct WeakCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallback : IWeak
    where TCallbackExecuter : ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, TCallbackExecuter>(argument, callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Purge(TCallback[] callbacks, ref int count)
        => CallbackExecuterHelper.Purge(callbacks, ref count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Dispose(TCallback[] callbacks, int count)
        => CallbackExecuterHelper.WeakDispose(callbacks, count);
}

internal static class CallbackExecuterHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke<TEvent, TCallback, TCallbackExecuter>(TEvent argument, TCallback[] callbacks, int count)
        where TCallbackExecuter : ICallbackExecuterSingle<TEvent, TCallback>
    {
        Debug.Assert(count > 0);
        Debug.Assert(callbacks.Length > count);

        ref TCallback current = ref Utils.GetArrayDataReference(callbacks);
        ref TCallback end = ref Unsafe.Add(ref current, count);

        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
#if NET7_0_OR_GREATER
            TCallbackExecuter
#else
            Utils.NullRef<TCallbackExecuter>()
#endif
                .Invoke(argument, current);
            current = ref Unsafe.Add(ref current, 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Purge<TCallback>(TCallback[] callbacks, ref int count)
        where TCallback : IWeak
    {
        TCallback[] callbacks_ = callbacks;
        int count_ = count;
        for (int i = 0; i < count_; i++)
        {
            if (callbacks_[i].FreeIfIsCollected())
            {
                do
                {
                    if (--count_ == i)
                        goto end;
                }
                while (callbacks_[count_].FreeIfIsCollected());
                callbacks_[i] = callbacks_[count_ + 1];
            }
        }
        end:
        count = count_;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WeakDispose<TCallback>(TCallback[] callbacks, int count)
       where TCallback : IWeak
    {
        if (count == 0)
            return;

        Debug.Assert(callbacks.Length > count);

        ref TCallback current = ref Utils.GetArrayDataReference(callbacks);
        ref TCallback end = ref Unsafe.Add(ref current, count);

        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            current.Free();
            current = ref Unsafe.Add(ref current, 1);
        }
    }
}
