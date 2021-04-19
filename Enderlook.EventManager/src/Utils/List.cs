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

        public T this[int index] => Array[index];

        public Array UnderlyingObject => Array.AsObject;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List(Array<T> array, int count)
        {
            Array = array;
            Count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> Clone()
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
        public void ConcurrentAddFrom(ref List<T> toAdd)
        {
            Array<T> self = Array<T>.Steal(ref Array);
            Array<T> add = Array<T>.Steal(ref toAdd.Array);

            if (toAdd.Count == 0)
            {
                Array<T>.Overwrite(ref Array, self);
                Array<T>.Overwrite(ref toAdd.Array, add);
                return;
            }

            int total = Count + toAdd.Count;
            if (self.Length < total)
                ResizeAndAdd(ref this, ref toAdd);
            else
                AddFrom(ref this, ref toAdd);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void AddFrom(ref List<T> selfList, ref List<T> toAdd)
            {
                add.CopyTo(self, selfList.Count + 1, toAdd.Count);
                add.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
                selfList.Count = total;
                Array<T>.Overwrite(ref selfList.Array, self);
                Array<T>.Overwrite(ref toAdd.Array, add);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void ResizeAndAdd(ref List<T> selfList, ref List<T> toAdd)
            {
                Array<T> newArray = Array<T>.Rent(total);
                self.CopyTo(newArray, selfList.Count);
                self.ClearIfContainsReferences(selfList.Count);
                self.Return();
                add.CopyTo(newArray, selfList.Count, toAdd.Count);
                add.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
                selfList.Count = total;
                Array<T>.Overwrite(ref selfList.Array, newArray);
                Array<T>.Overwrite(ref toAdd.Array, add);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ConcurrentRemoveFrom(ref List<T> toRemove)
        {
            Array<T> self = Array<T>.Steal(ref Array);
            Array<T> remove = Array<T>.Steal(ref toRemove.Array);

            if (toRemove.Count == 0)
            {
                Array<T>.Overwrite(ref Array, self);
                Array<T>.Overwrite(ref toRemove.Array, remove);
                return;
            }

            Remove(ref this, ref toRemove);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Remove(ref List<T> selfList, ref List<T> removeList)
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                // TODO: Time complexity of this could be reduced by sorting the arrays. Research if that may be worth.
                int j = 0;
                T _ = self[selfList.Count - 1];
                _ = remove[removeList.Count - 1];
                for (int i = 0; i < selfList.Count; i++)
                {
                    T element = self[i];
                    for (int k = removeList.Count - 1; k >= 0; k--)
                    {
                        if (comparer.Equals(element, remove[k]))
                        {
                            remove.CopyTo(k + 1, remove, k, removeList.Count - k);
                            removeList.Count--;
                            goto next;
                        }
                        self[j++] = element;
                        next:;
                    }
                }
                selfList.Count = j;

                remove.ClearIfContainsReferences(removeList.Count);
                removeList.Count = 0;

                Array<T>.Overwrite(ref selfList.Array, self);
                Array<T>.Overwrite(ref removeList.Array, remove);
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