using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        // We don't apply AggressiveInlining because we always use it as a cold path.
        public List<T> Clone()
        {
            List<T> clone = Rent(Count);
            Array.CopyTo(clone.Array, Count);
            clone.Count = Count;
            return clone;
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
        public void Add(T element)
        {
            if (Count == Array.Length)
                ResizeAndAdd(ref this);
            else
                Array[Count++] = element;

            [MethodImpl(MethodImplOptions.NoInlining)]
            void ResizeAndAdd(ref List<T> self)
            {
                if (self.Count == 0)
                    self.Array = Array<T>.Rent(INITIAL_CAPACITY);
                else
                {
                    Array<T> newArray = Array<T>.Rent(self.Count * GROW_FACTOR);
                    self.Array.CopyTo(newArray, self.Count);
                    self.Array.ClearIfContainsReferences(self.Count);
                    self.Array.Return();
                    self.Array = newArray;
                }

                self.Array[self.Count++] = element;
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
                if (typeof(IWeak).IsAssignableFrom(typeof(T)))
                {
                    for (int i = 0; i < toAdd.Count; i++)
                    {
                        T element = toAdd[i];
                        // TODO: We could avoid the safe casting.
                        GCHandle handle = ((IWeak)element).Handle;
                        if (handle.Target is not null)
                            self.Add(element);
                        else
                            handle.Free();
                    }
                }
                else
                {
                    toAdd.Array.CopyTo(self.Array, self.Count + 1, toAdd.Count);
                    self.Count = total;
                }
                toAdd.Array.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void ResizeAndAdd(ref List<T> self, ref List<T> toAdd)
            {
                Array<T> newArray = Array<T>.Rent(total);
                self.Array.CopyTo(newArray, self.Count);
                self.Return();

                if (typeof(IWeak).IsAssignableFrom(typeof(T)))
                {
                    self = new(newArray, self.Count);
                    for (int i = 0; i < toAdd.Count; i++)
                    {
                        T element = toAdd[i];
                        // TODO: We could avoid the safe casting.
                        GCHandle handle = ((IWeak)element).Handle;
                        if (handle.Target is not null)
                            self.Add(element);
                        else
                            handle.Free();
                    }
                }
                else
                {
                    toAdd.Array.CopyTo(newArray, self.Count, toAdd.Count);
                    self = new List<T>(newArray, total);
                }
                toAdd.Array.ClearIfContainsReferences(toAdd.Count);
                toAdd.Count = 0;
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
                        T element2 = toRemove[k];
                        if (comparer.Equals(element, element2))
                        {
                            if (typeof(IWeak).IsAssignableFrom(typeof(T)))
                            {
                                // TODO: We could avoid the safe casting.
                                ((IWeak)element2).Handle.Free();
                                ((IWeak)element).Handle.Free();
                            }
                            toRemove.Array.CopyTo(k + 1, toRemove.Array, k, toRemove.Count - k);
                            toRemove.Count--;
                        }
                        else
                            self[j++] = element;
                    }
                }
                self.Count = j;

                if (typeof(IWeak).IsAssignableFrom(typeof(T)))
                {
                    for (int i = 0; i < toRemove.Count; i++)
                        // TODO: We could avoid the safe casting.
                        ((IWeak)toRemove[i]).Handle.Free();
                }

                toRemove.Array.ClearIfContainsReferences(toRemove.Count);
                toRemove.Count = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveWeakHandlesIfHas()
        {
            if (typeof(IWeak).IsAssignableFrom(typeof(T)))
            {
                int j = 0;
                for (int i = 0; i < Count; i++)
                {
                    T element = Array[i];
                    // TODO: We could avoid the safe casting.
                    GCHandle handle = ((IWeak)element).Handle;
                    if (handle.Target is not null)
                        Array[j++] = element;
                    else
                        handle.Free();
                }
                Array.ClearIfContainsReferences(j, Count - j);
                Count = j;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return()
        {
            Array.ClearIfContainsReferences(Count);
            Array.Return();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (typeof(IWeak).IsAssignableFrom(typeof(T)) && Count > 0)
            {
                T _ = Array[Count - 1];
                for (int i = 0; i < Count; i++)
                    ((IWeak)Array[i]).Handle.Free();
            }
            Array.ClearIfContainsReferences(Count);
            Array.Return();
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