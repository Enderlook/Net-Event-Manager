using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal static class ArrayUtils
{
    public const int INITIAL_CAPACITY = 16;
    private const int GROW_FACTOR = 2;
    public const float SHRINK_FACTOR_THRESHOLD = .25f;

    public static void ConcurrentAdd<T>(ref T[]? array, ref int count, T element)
    {
        T[] takenArray = Utils.Take(ref array);
        {
            if (unchecked((uint)count < (uint)takenArray.Length))
            {
                takenArray[count++] = element;
                Utils.Untake(ref array, takenArray);
            }
            else
                AddWithResize(ref array, ref count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void AddWithResize(ref T[]? array, ref int count)
        {
            T[] takenArray_ = takenArray;
            int count_ = count;
            if (count_ == 0)
            {
                Debug.Assert(takenArray_.Length == 0);
                takenArray_ = InitialArray<T>();
                takenArray_[0] = element;
                count = 1;
                Utils.Untake(ref array, takenArray_);
            }
            else
            {
                Debug.Assert(count_ == takenArray_.Length);
                T[] newArray = RentArray<T>(count_ * GROW_FACTOR);
                Array.Copy(takenArray_, newArray, count_);
                newArray[count_] = element;
                count = count_ + 1;
                Utils.Untake(ref array, newArray);
                ReturnArray(takenArray_, count_);
            }
        }
    }

    public static void Add<T>(ref T[] array, ref int count, T element)
    {
        T[] array_ = array;
        {
            if (unchecked((uint)count < (uint)array_.Length))
                array_[count++] = element;
            else
                AddWithResize(ref array, ref count);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void AddWithResize(ref T[] array, ref int count)
        {
            T[] array__ = array_;
            int count_ = count;
            if (count_ == 0)
            {
                Debug.Assert(array__.Length == 0);
                array__ = InitialArray<T>();
                array__[0] = element;
                count = 1;
                array = array__;
            }
            else
            {
                Debug.Assert(count_ == array__.Length);
                T[] newArray = RentArray<T>(count_ * GROW_FACTOR);
                Array.Copy(array__, newArray, count_);
                ReturnArray(array__, count_);
                newArray[count_] = element;
                count = count_ + 1;
                array = newArray;
            }
        }
    }

    public static void ConcurrentRemove<TComparer, TElement>(ref TElement[]? array, ref int count, TComparer comparer)
        where TComparer : IPredicator<TElement>
    {
        int count_ = count;
        TElement[] takenArray = Utils.Take(ref array);
        {
            if (count_ == 0)
            {
                Utils.Untake(ref array, takenArray);
                return;
            }

            Debug.Assert(takenArray.Length > count_);

            ref TElement current = ref Utils.GetArrayDataReference(takenArray);
            ref TElement end = ref Unsafe.Add(ref current, count_);
            for (int i = 0; Unsafe.IsAddressLessThan(ref current, ref end); i++)
            {
                if (comparer.DoesMatch(current))
                {
                    Array.Copy(takenArray, i + 1, takenArray, i, count - i);

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TElement>())
#endif
                    takenArray[count] = default!;
                    count = count_ - 1;
                    Utils.Untake(ref array, takenArray);
                    return;
                }
                current = ref Unsafe.Add(ref current, 1);
            }

            Utils.Untake(ref array, takenArray);
        }
    }

    public static bool TryShrink<T>(ref T[] array, int count)
    {
        Debug.Assert((1f / GROW_FACTOR) > SHRINK_FACTOR_THRESHOLD);

        T[] array_ = array;
        int originalSize = array_.Length;
        int size = originalSize;

        if (count != 0 || array_.Length == 0)
        {
            float size_ = size;
            while ((count / size_) < SHRINK_FACTOR_THRESHOLD && size > INITIAL_CAPACITY)
                size /= GROW_FACTOR;

            if (size != originalSize)
            {
                T[] newArray = RentArray<T>(size);
                Debug.Assert(newArray.Length < array_.Length);
                Array.Copy(array_, newArray, count);
                array = newArray;
                ReturnArray(array_, count);
                return true;
            }

            return false;
        }

        ReturnArray(array_);
        array = EmptyArray<T>();
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] InitialArray<T>() => RentArray<T>(INITIAL_CAPACITY);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] EmptyArray<T>() => RentArray<T>(0);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] RentArray<T>(int minimumCapacity) => ArrayPool<T>.Shared.Rent(minimumCapacity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnArray<T>(T[] array) => ArrayPool<T>.Shared.Return(array);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnArray<T>(T[] array, int count)
    {
        ClearReferences(array, count);
        ReturnArray(array);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Clone<T>(T[] array, int count)
    {
        T[] newArray = RentArray<T>(count);
        Array.Copy(array, newArray, count);
        return newArray;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClearReferences<T>(T[] array, int count)
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
#endif
            Array.Clear(array, 0, count);
    }
}
