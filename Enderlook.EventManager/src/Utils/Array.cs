﻿using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct Array<T>
    {
        internal object array;

        private T[] AsArray {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.As<T[]>(array);
        }

        public object AsObject {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array;
        }

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsArray[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => AsArray[index] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Array(T[] array)
        {
            if (typeof(T).IsValueType)
                Debug.Assert(array.GetType() == typeof(T[]));
            else
                Debug.Assert(array.GetType() == typeof(object[]));

            this.array = array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Array(object array)
        {
            if (typeof(T).IsValueType)
                Debug.Assert(array.GetType() == typeof(T[]));
            else
                Debug.Assert(array.GetType() == typeof(object[]));

            this.array = array;
        }

        public int Length {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Unsafe.As<T[]>(array).Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Array<T> otherArray, int count)
            => Array.Copy(AsArray, otherArray.AsArray, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Array<T> to, int toStartIndex, int count)
            => Array.Copy(AsArray, 0, to.AsArray, toStartIndex, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(int startIndex, Array<T> to, int toStartIndex, int count)
            => Array.Copy(AsArray, startIndex, to.AsArray, toStartIndex, count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyToAndReturn(Array<T> otherArray, int count)
        {
            CopyTo(otherArray, count);
            ClearAndReturn(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Clear(int count) => Array.Clear(AsArray, 0, count);

        private static class Container<U>
        {
            public static readonly U[] array = new U[0];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Array<T> Empty()
        {
            // We reduce amount of generic instantiation of ArrayPool<T> by sharing reference type.
            // Take into account that this is quite dangerous, as myArray.GetType() will return object[].
            // However we never do that.
            if (typeof(T).IsValueType)
                return new Array<T>(Container<T>.array);
            else
                return new Array<T>(Unsafe.As<T[]>(Container<object>.array));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Array<T> Rent(int minCapacity)
        {
            // We reduce amount of generic instantiation of ArrayPool<T> by sharing reference type.
            // Take into account that this is quite dangerous, as myArray.GetType() will return object[].
            // However we never do that.
            if (typeof(T).IsValueType)
                return new Array<T>(ArrayPool<T>.Shared.Rent(minCapacity));
            else
                return new Array<T>(Unsafe.As<T[]>(ArrayPool<object>.Shared.Rent(minCapacity)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return()
        {
            // We reduce amount of generic instantiation of ArrayPool<T> by sharing reference type.
            // Take into account that this is quite dangerous, as myArray.GetType() will return object[].
            // However we never do that.
            if (typeof(T).IsValueType)
            {
                Debug.Assert(array.GetType() == typeof(T[]));
                ArrayPool<T>.Shared.Return(Unsafe.As<T[]>(array));
                array = Empty();
            }
            else
            {
                Debug.Assert(array.GetType() == typeof(object[]));
                ArrayPool<object>.Shared.Return(Unsafe.As<object[]>(array));
                array = Empty();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearAndReturn(int count)
        {
            // TODO, in .Net Standard 2.1 we can use RuntimeHelpers.IsReferenceOrContainsReferences<T>() to check if we need to clear it or not.
            Clear(count);
            Return();
        }
    }
}