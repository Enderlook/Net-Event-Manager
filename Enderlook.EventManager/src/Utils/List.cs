using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct List<T>
    {
        private const int INITIAL_CAPACITY = 4;
        private const int GROW_FACTOR = 2;

        public Array<T> Array;

        public int Count { get; private set; }

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[index] = value;
        }

        public Array UnderlyingObject {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array.AsObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List(Array<T> array, int count)
        {
            Array = array;
            Count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Steal(ref List<T> list)
        {
            Array<T> stolen = Array<T>.Steal(ref list.Array);
            return new List<T>(stolen, list.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Overwrite(ref List<T> list, List<T> other)
        {
            list.Count = other.Count;
            Array<T>.Overwrite(ref list.Array, other.Array);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> ConcurrentClone()
        {
            Array<T> stolenArray = Array<T>.Steal(ref Array);
            List<T> list = Rent(Count);
            stolenArray.CopyTo(list.Array, Count);
            Array<T>.Overwrite(ref Array, stolenArray);
            list.Count = Count;
            return list;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConcurrentAdd(T element)
        {
            Array<T> stolenArray = Array<T>.Steal(ref Array);
            int count_ = Count;
            if (count_ == stolenArray.Length)
                ResizeAndAdd(ref this, count_);
            else
            {
                stolenArray[count_++] = element;
                Count = count_;
                Array<T>.Overwrite(ref Array, stolenArray);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void ResizeAndAdd(ref List<T> self, int count_)
            {
                if (count_ == 0)
                    stolenArray = Array<T>.Rent(INITIAL_CAPACITY);
                else
                {
                    Array<T> newArray = Array<T>.Rent(count_ * GROW_FACTOR);
                    stolenArray.CopyTo(newArray, count_);
                    stolenArray.ClearIfContainsReferences(count_);
                    stolenArray.Return();
                    stolenArray = newArray;
                }

                stolenArray[count_++] = element;
                self.Count = count_;
                Array<T>.Overwrite(ref self.Array, stolenArray);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InjectZero()
        {
            Debug.Assert(Array.AsObject is null);
            Count = 0;
            Array = Array<T>.Empty();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddFrom(ref List<T> toAdd)
        {
            if (toAdd.Count == 0)
                return;

            int total = Count + toAdd.Count;
            if (Array.Length < total)
                ResizeAndAdd(ref this, ref toAdd);
            else
                AddFrom(ref this, ref toAdd);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void AddFrom(ref List<T> self, ref List<T> toAdd)
            {
                toAdd.Array.CopyTo(self.Array, self.Count + 1, toAdd.Count);
                toAdd.Array.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
                self.Count = total;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void ResizeAndAdd(ref List<T> self, ref List<T> toAdd)
            {
                Array<T> newArray = Array<T>.Rent(total);
                self.Array.CopyTo(newArray, self.Count);
                self.Return();
                toAdd.Array.CopyTo(newArray, self.Count, toAdd.Count);
                toAdd.Array.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
                self = new List<T>(newArray, total);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFrom(ref List<T> toRemove)
        {
            if (toRemove.Count == 0)
                return;

            Remove(ref this, ref toRemove);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static void Remove(ref List<T> self, ref List<T> toRemove)
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                T _ = self[self.Count - 1];
                _ = toRemove[toRemove.Count - 1];
                for (int i = 0; i < self.Count; i++)
                {
                    T element = self[i];
                    for (int k = toRemove.Count - 1; k >= 0; k--)
                    {
                        if (comparer.Equals(element, toRemove[k]))
                        {
                            toRemove.Array.CopyTo(k + 1, toRemove.Array, k, toRemove.Count - k);
                            toRemove.Count--;
                            goto next;
                        }
                        self[j++] = element;
                        next:;
                    }
                }
                self.Count = j;

                toRemove.Array.ClearIfContainsReferences(toRemove.Count);
                toRemove.Count = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return()
        {
            Array.ClearIfContainsReferences(Count);
            Array.Return();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExtractIfEmpty()
        {
            Array<T> self = Array<T>.Steal(ref Array);
            if (Count == 0)
            {
                self.Return();
                Array = Array<T>.Empty();
            }
            else
                Array<T>.Overwrite(ref Array, self);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Rent(int minCapacity) => new(Array<T>.Rent(minCapacity), 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Empty() => new(Array<T>.Empty(), 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T ConcurrentGet(int index)
        {
            Array<T> self = Array<T>.Steal(ref Array);
            Debug.Assert(index < Count);
            T element = self[index];
            Array<T>.Overwrite(ref Array, self);
            return element;
        }
    }
}