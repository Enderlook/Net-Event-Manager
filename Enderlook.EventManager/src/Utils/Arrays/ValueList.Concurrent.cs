using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal partial struct ValueList<T>
    {
        public bool IsLocked {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count == LOCK;
        }

        public T[] ArrayFromLocked {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Debug.Assert(IsLocked);
                return array;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConcurrentAdd(T element) => AddAndUnlock(LockAndGetCount(), element);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddAndUnlock(int count, T element)
        {
            T[] array_ = array;
            if ((uint)count < (uint)array_.Length)
            {
                array_[count++] = element;
                this.count = count;
            }
            else
                AddWithResize(count, element);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(int count_, T element)
        {
            if (count_ == 0)
            {
                T[] newArray = RentArray(INITIAL_CAPACITY);
                newArray[count_] = element;
                array = newArray;
                count = count_ + 1;
            }
            else
            {
                Debug.Assert(count_ == array.Length);

                T[] array_ = array;
                T[] newArray = RentArray(count_ * GROW_FACTOR);
                Array.Copy(array_, newArray, count_);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                    Array.Clear(array_, 0, count_);

                ReturnArray(array_);

                newArray[count_] = element;
                array = newArray;
                count = count_ + 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConcurrentRemove(T element)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;

            int count_ = LockAndGetCount();
            T[] array_ = array;

            if ((uint)count_ >= (uint)array_.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < count_; i++)
            {
                T element2 = array_[i];
                if (typeof(T).IsValueType ? EqualityComparer<T>.Default.Equals(element2, element) : comparer.Equals(element2, element))
                {
                    int newCount = count_ - i;
                    Array.Copy(array_, i + 1, array_, i, newCount);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                        array_[count_] = default;
                    Unlock(count_);
                    return;
                }
            }
            Unlock(count_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LockAndGetCount()
        {
            int count_;
            do
            {
                count_ = Interlocked.Exchange(ref count, LOCK);
            } while (count_ == LOCK);
            return count_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ConcurrentCount()
        {
            int count_;
            do
            {
                count_ = count;
            } while (count == LOCK);
            return count_;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueList<T> ConcurrentClone()
        {
            int count_ = LockAndGetCount();
            ValueList<T> list = Create(count_);
            list.count = count_;
            Array.Copy(array, list.array, count_);
            Unlock(count_);
            return list;
        }

        public void InitializeWithAndUnlock(T element)
        {
            T[] array_ = RentArray(INITIAL_CAPACITY);
            array_[0] = element;
            array = array_;
            count = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueList<T> Lock() => new() { count = LockAndGetCount(), array = array };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueList<T> WithLock(int count) => new() { count = count, array = array };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unlock(ValueList<T> list)
        {
            array = list.array;
            count = list.count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unlock(int count) => this.count = count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ConcurrentTryMoveNext(ref int index, out T element)
        {
            int count_ = LockAndGetCount();
            if (index < count_)
            {
                element = array[index];
                count = count_;
                index++;
                return true;
            }
            count = count_;
            element = default;
            return false;
        }
    }
}
