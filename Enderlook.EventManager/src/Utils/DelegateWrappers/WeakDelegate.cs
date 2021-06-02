using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct WeakDelegate<TDelegate> : IEquatable<WeakDelegate<TDelegate>>
        where TDelegate : IEquatable<TDelegate>
    {
        private WeakReference<object> reference;
        public readonly TDelegate callback;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public WeakDelegate(WeakReference<object> reference, TDelegate callback)
        {
            this.callback = callback;
            this.reference = reference;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasExpired() => !reference.TryGetTarget(out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetHandle([NotNullWhen(true)] out object? handle) => reference.TryGetTarget(out handle);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(WeakDelegate<TDelegate> other)
        {
            if (!callback.Equals(other.callback))
                return false;
            if (reference.TryGetTarget(out object? target))
            {
                if (other.reference.TryGetTarget(out object? otherTarget))
                    return ReferenceEquals(target, otherTarget);
                else
                    return false;
            }
            if (other.reference.TryGetTarget(out _))
                return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRemove(ref ValueList<WeakDelegate<TDelegate>> list, object handle, TDelegate callback)
        {
            EqualityComparer<TDelegate> delegateComparer = EqualityComparer<TDelegate>.Default;

            int count_ = list.LockAndGetCount();
            WeakDelegate<TDelegate>[] array_ = list.ArrayFromLocked;

            if (unchecked((uint)count_ >= (uint)array_.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            for (int i = 0; i < count_; i++)
            {
                WeakDelegate<TDelegate> element2 = array_[i];

                if (!element2.TryGetHandle(out object? handle_))
                    continue;

                if (ReferenceEquals(handle_, handle))
                {
                    if (typeof(TDelegate).IsValueType ? EqualityComparer<TDelegate>.Default.Equals(element2.callback, callback) : delegateComparer.Equals(element2.callback, callback))
                    {
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
        public static void FreeExpired(ref ValueList<WeakDelegate<TDelegate>> list)
        {
            int count_ = list.Count;
            Debug.Assert(count_ != -1);
            WeakDelegate<TDelegate>[] array_ = list.ArrayUnlocked;

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
                WeakDelegate<TDelegate> element = array_[i];
                if (element.HasExpired())
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