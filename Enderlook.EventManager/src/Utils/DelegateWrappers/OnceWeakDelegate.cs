using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal struct OnceWeakDelegate<TDelegate> : IEquatable<OnceWeakDelegate<TDelegate>>
        where TDelegate : IEquatable<TDelegate>
    {
        private GCHandle handle;
        public readonly TDelegate callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OnceWeakDelegate(GCHandle handle, TDelegate callback)
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
        private bool FreeIfExpired()
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
        public bool TryGetHandle([NotNullWhen(true)] out object? handle)
        {
            handle = this.handle.Target;
            return handle is not null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetHandleOrFreeIfExpired([NotNullWhen(true)] out object? handle)
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
        public bool Equals(OnceWeakDelegate<TDelegate> other) => callback.Equals(other.callback) && ReferenceEquals(handle.Target, other.handle.Target);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRemove(ref ValueList<OnceWeakDelegate<TDelegate>> list, object handle, TDelegate callback)
        {
            EqualityComparer<TDelegate> delegateComparer = EqualityComparer<TDelegate>.Default;

            int count_ = list.LockAndGetCount();
            OnceWeakDelegate<TDelegate>[] array_ = list.ArrayFromLocked;

            if (unchecked((uint)count_ >= (uint)array_.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            for (int i = 0; i < count_; i++)
            {
                OnceWeakDelegate<TDelegate> element2 = array_[i];

                if (!element2.TryGetHandleOrFreeIfExpired(out object? handle_))
                    continue;

                if (ReferenceEquals(handle_, handle))
                {
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
                array_[j++] = element2;
            }
            list.Unlock(j);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FreeExpired(ref ValueList<OnceWeakDelegate<TDelegate>> list)
        {
            int count_ = list.Count;
            Debug.Assert(count_ != -1);
            OnceWeakDelegate<TDelegate>[] array_ = list.ArrayUnlocked;

            if (unchecked((uint)count_ >= array_.Length))
            {
                if (count_ == 0)
                    return;
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            for (int i = 0; i < count_; i++)
            {
                OnceWeakDelegate<TDelegate> element = array_[i];
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