using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager;

/// <summary>
/// A simple non-reentrant lock.
/// </summary>
public struct SpinLock
{
    private volatile int acquired;

    /// <summary>
    /// Returns <see langword="true"/> if the lock is acquired, else <see langword="false"/>.
    /// </summary>
#pragma warning disable CS0420 // A reference to a volatile field will not be treated as volatile. `Unsafe.As<,>` doesn't read.
    public bool IsAcquired
    {
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get => Volatile.Read(ref Unsafe.As<int, bool>(ref acquired));
    }
#pragma warning restore CS0420 // A reference to a volatile field will not be treated as volatile

    /// <summary>
    /// Enter the lock.
    /// </summary>
    /// <param name="taken">If the method returns, this will be always <see langword="true"/>.<br/>
    /// If an exception ocurrs, the parameter will be asigned the value <see langword="true"/> only if the lock was taken.<br/>
    /// Otherwise, no value is assigned.</param>
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Enter(ref bool taken)
    {
        while (TryAcquire())
            /* This is empty on purpose. */;
        taken = true;
    }

    /// <summary>
    /// Try enter the lock if it's not already acquired.
    /// </summary>
    /// <param name="taken">If the method returns, this determines if the lock was taken.<br/>
    /// If an exception ocurrs, the parameter will be asigned the value <see langword="true"/> only if the lock was taken, otherwise, no value is assigned.</param>
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void TryEnter(ref bool taken) => taken = TryAcquire();

    /// <summary>
    /// Try enter the lock if it's not already acquired.
    /// </summary>
    /// <param name="iterations">The number of attempts to acquire the lock before returning without it.</param>
    /// <param name="taken">If the method returns, this determines if the lock was taken.<br/>
    /// If an exception ocurrs, the parameter will be asigned the value <see langword="true"/> only if the lock was taken, otherwise, no value is assigned.</param>
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void TryEnter(uint iterations, ref bool taken)
    {
        while (TryAcquire())
        {
            if (unchecked(iterations--) == 0) // decrease after check so an initial value of 0 to `iterations` works.
            {
                return;
            }
        }
        taken = true;
    }

    /// <summary>
    /// Try enter the lock if it's not already acquired.
    /// </summary>
    /// <param name="timeout">The <see cref="TimeSpan"/> to attempt to acquire the lock defore returning without it.<br/>
    /// A negative <see cref="TimeSpan"/> causes undefined behaviour.</param>
    /// <param name="taken">If the method returns, this determines if the lock was taken.<br/>
    /// If an exception ocurrs, the parameter will be asigned the value <see langword="true"/> only if the lock was taken, otherwise, no value is assigned.</param>
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void TryEnter(TimeSpan timeout, ref bool taken)
    {
        long end = unchecked((long)timeout.TotalMilliseconds * Stopwatch.Frequency + Stopwatch.GetTimestamp());
        while (TryAcquire())
        {
            if (Stopwatch.GetTimestamp() >= end)
            {
                return;
            }
        }
        taken = true;
    }

    /// <summary>
    /// Exit the lock.<br/>
    /// This should only be called if you have ownership of the lock.<br/>
    /// Otherwise it leads to undefined behaviour.
    /// </summary>
#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    public void Exit() => acquired = 0; // This is always atomic.

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private bool TryAcquire() => Interlocked.CompareExchange(ref acquired, 1, 0) != 0;
}
