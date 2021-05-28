using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal partial struct ValueList<T>
    {
        private const int INITIAL_CAPACITY = 16;
        private const int GROW_FACTOR = 2;
        private const int SHRINK_FACTOR = 4;
        private const int SHRINK_THRESHOLD = 8;
        private const int LOCK = -1;

        private T[] array;
        private int count;

        public bool IsDefault {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array is null;
        }

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                int count_ = count;
                Debug.Assert(count_ != LOCK);
                return count_;
            }
        }

        public T[] ArrayUnlocked {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                Debug.Assert(!IsLocked);
                return array;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ValueList(T[] array)
        {
            this.array = array;
            count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueList<T> Create(int minimumCapacity = 0) => new(RentArray(minimumCapacity));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(int index)
        {
            int count_ = count;
            Debug.Assert(count_ != LOCK);
            Debug.Assert(index < count_);
            return array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, T element)
        {
            int count_ = count;
            Debug.Assert(count_ != LOCK);
            Debug.Assert(index < count_);
            array[index] = element;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T element)
        {
            int count_ = count;
            Debug.Assert(count_ != LOCK);
            AddAndUnlock(count_, element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Debug.Assert(!IsLocked);
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                Array.Clear(array, 0, count);
            count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return()
        {
            Debug.Assert(!IsLocked);
            ReturnArray(array);
            array = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryShrink()
        {
            Debug.Assert(!IsLocked);
            if (count / array.Length >= SHRINK_THRESHOLD)
            {
                T[] newArray = RentArray(array.Length / SHRINK_FACTOR);
                if (newArray.Length == array.Length)
                    ReturnArray(newArray);

                Array.Copy(array, 0, newArray, 0, count);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                    Array.Clear(array, 0, count);

                array = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Slice ToSlice()
        {
            Debug.Assert(!IsLocked);
            return new(array, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueList<T> Clone()
        {
            Debug.Assert(!IsLocked);
            ValueList<T> list = Create(count);
            list.count = count;
            Array.Copy(array, 0, list.array, 0, count);
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(Slice slice)
        {
            Debug.Assert(typeof(T).IsValueType ? slice.array.GetType() == typeof(T[]) : slice.array.GetType() == typeof(object[]));
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
                Array.Clear(slice.array, 0, slice.count);
            ReturnArray(CastUtils.ExpectExactType<T[]>(slice.array));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] RentArray(int minimumCapacity) => ArrayPool<T>.Shared.Rent(minimumCapacity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ReturnArray(T[] array) => ArrayPool<T>.Shared.Return(array);
    }
}