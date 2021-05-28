using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal readonly struct EquatableDelegate : IEquatable<EquatableDelegate>
    {
        // Delegate type is erased to avoid generic instantiation of too many ArrayPool<T>.

        public readonly Delegate callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EquatableDelegate(Delegate callback) => this.callback = callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EquatableDelegate other) => callback.Equals(other.callback);
    }

    internal readonly struct DelegateWithClosure<TClosure> : IEquatable<DelegateWithClosure<TClosure>>
    {
        // Delegate type is erased to avoid generic instantiation of too many ArrayPool<T>.

        private static readonly EqualityComparer<TClosure> closureComparer = EqualityComparer<TClosure>.Default;

        public readonly Delegate callback;
        public readonly TClosure closure;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DelegateWithClosure(Delegate callback, TClosure closure)
        {
            this.callback = callback;
            this.closure = closure;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(DelegateWithClosure<TClosure> other)
            => callback.Equals(other.callback) && typeof(TClosure).IsValueType ?
            EqualityComparer<TClosure>.Default.Equals(closure, other.closure) :
            closureComparer.Equals(closure, other.closure);
    }

    internal partial struct WeakDelegate<TDelegate> : IEquatable<WeakDelegate<TDelegate>>, IWeak
        where TDelegate : IEquatable<TDelegate>
    {
        private GCHandle handle;
        public readonly TDelegate callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WeakDelegate(GCHandle handle, TDelegate callback)
        {
            this.callback = callback;
            this.handle = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            Debug.Assert(handle.IsAllocated);
            handle.Free();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FreeIfExpired()
        {
            Debug.Assert(handle.IsAllocated);
            if (handle.Target is null)
            {
                handle.Free();
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetHandle(out object handle)
        {
            handle = this.handle.Target;
            return handle is not null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetHandleFreeIfExpired(out object handle)
        {
            handle = this.handle.Target;

            if (handle is null)
            {
                this.handle.Free();
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(WeakDelegate<TDelegate> other) => callback.Equals(other.callback) && ReferenceEquals(handle.Target, other.handle.Target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRemove<THandle>(ref ValueList<WeakDelegate<TDelegate>> list, THandle handle, TDelegate callback)
            where THandle : class
        {
            EqualityComparer<TDelegate> delegateComparer = EqualityComparer<TDelegate>.Default;
            EqualityComparer<THandle> handleComparer = EqualityComparer<THandle>.Default;

            int count_ = list.LockAndGetCount();
            WeakDelegate<TDelegate>[] array_ = list.ArrayFromLocked;

            if ((uint)count_ >= (uint)array_.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            for (int i = 0; i < count_; i++)
            {
                WeakDelegate<TDelegate> element2 = array_[i];

                if (!element2.TryGetHandleFreeIfExpired(out object handle_))
                    continue;

                if (!handleComparer.Equals(CastUtils.ExpectExactType<THandle>(handle_), handle))
                {
                    array_[j++] = element2;
                    continue;
                }

                if (typeof(TDelegate).IsValueType ? EqualityComparer<TDelegate>.Default.Equals(element2.callback, callback) : delegateComparer.Equals(element2.callback, callback))
                {
                    element2.Free();

                    int newCount = count_ - j;
                    Array.Copy(array_, i + 1, array_, j, newCount);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<WeakDelegate<TDelegate>>())
#endif
                        array_[count_] = default;
                    list.Unlock(count_);
                    return;
                }
            }
            list.Unlock(j);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FreeExpired(ref ValueList<WeakDelegate<TDelegate>> list)
        {
            int count_ = list.Count;
            Debug.Assert(count_ != -1);
            WeakDelegate<TDelegate>[] array_ = list.ArrayUnlocked;

            if ((uint)count_ >= array_.Length)
            {
                if (count_ == 0)
                    return;
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            for (int i = 0; i < count_; i++)
            {
                WeakDelegate<TDelegate> element = array_[i];
                if (element.FreeIfExpired())
                    continue;
                array_[j++] = element;
            }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (RuntimeHelpers.IsReferenceOrContainsReferences<WeakDelegate<TDelegate>>())
#endif
                Array.Clear(array_, j, count_ - j);
            count_ = j;
        }
    }
}