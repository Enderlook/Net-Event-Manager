using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal interface ICallbackExecuter<TEvent, TCallback>
{
    void Invoke(TEvent argument, TCallback[] callbacks, int count);

    bool IsOnce();

    void Purge(ref TCallback[] callbacks, ref int count);

    void Dispose(TCallback[] callbacks, int count);
}

internal interface IWeak
{
    void Free();

    bool FreeIfIsCollected();
}

internal readonly struct ExecuteAndFree<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuterSingle<TEvent, TCallback>
    where TCallback : IWeak
    where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TEvent argument, TCallback callback)
    {
        Utils.Null<TCallbackExecuter>().Invoke(argument, callback);
        callback.Free();
    }
}

internal struct StrongMultipleCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, TCallbackExecuter>(argument, callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnce() => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Purge(ref TCallback[] callbacks, ref int count)
        => ArrayUtils.TryShrink(ref callbacks, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose(TCallback[] callbacks, int count) { }
}

internal struct WeakMultipleCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallback : IWeak
    where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, TCallbackExecuter>(argument, callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnce() => false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Purge(ref TCallback[] callbacks, ref int count)
        => CallbackExecuterHelper.Purge(ref callbacks, ref count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose(TCallback[] callbacks, int count)
        => CallbackExecuterHelper.WeakDispose(callbacks, count);
}

internal struct StrongOnceCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, TCallbackExecuter>(argument, callbacks, count);
        ArrayUtils.ReturnArray(callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnce() => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Purge(ref TCallback[] callbacks, ref int count)
        => ArrayUtils.TryShrink(ref callbacks, count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose(TCallback[] callbacks, int count) { }
}

internal struct WeakOnceCallbackExecuter<TEvent, TCallback, TCallbackExecuter> : ICallbackExecuter<TEvent, TCallback>
    where TCallback : IWeak
    where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Invoke(TEvent argument, TCallback[] callbacks, int count)
    {
        if (count == 0)
            return;

        CallbackExecuterHelper.Invoke<TEvent, TCallback, ExecuteAndFree<TEvent, TCallback, TCallbackExecuter>>(argument, callbacks, count);
        ArrayUtils.ReturnArray(callbacks, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOnce() => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Purge(ref TCallback[] callbacks, ref int count)
        => CallbackExecuterHelper.Purge(ref callbacks, ref count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose(TCallback[] callbacks, int count)
        => CallbackExecuterHelper.WeakDispose(callbacks, count);
}

internal static class CallbackExecuterHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Invoke<TEvent, TCallback, TCallbackExecuter>(TEvent argument, TCallback[] callbacks, int count)
        where TCallbackExecuter : struct, ICallbackExecuterSingle<TEvent, TCallback>
    {
        Debug.Assert(count > 0);
        Debug.Assert(callbacks.Length > count);

        ref TCallback current = ref Utils.GetArrayDataReference(callbacks);
        ref TCallback end = ref Unsafe.Add(ref current, count);

        while (Unsafe.IsAddressLessThan(ref current, ref end))
        {
            Utils.Null<TCallbackExecuter>().Invoke(argument, current);
            current = ref Unsafe.Add(ref current, 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Purge<TCallback>(ref TCallback[] callbacks, ref int count)
        where TCallback : IWeak
    {
        TCallback[] callbacks_ = callbacks;
        int count_ = count;
        int oldCount = count_;
        for (int i = 0; i < count_; i++)
        {
            if (callbacks_[i].FreeIfIsCollected())
            {
                while (true)
                {
                    if (--count_ == i)
                        goto end;
                    if (!callbacks_[count_].FreeIfIsCollected())
                        break;
                }
                callbacks_[i] = callbacks_[count_ + 1];
            }
        }
        end:
        count = count_;
        if (count_ == 0)
        {
            ArrayUtils.ReturnArray(callbacks_, oldCount);
            callbacks = ArrayUtils.EmptyArray<TCallback>();
            return true;
        }
        else
        {
            if (!ArrayUtils.TryShrink(ref callbacks_, count_) && count_ != oldCount)
                Array.Clear(callbacks_, count_, oldCount - count_);
            callbacks = callbacks_;
            return false;
        }
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
