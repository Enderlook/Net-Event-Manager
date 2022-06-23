using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager;

internal struct Dictionary2<TKey, TValue>
{
    // Based from System.Collections.Generic.Dictionary<TKey, TValue> and https://github.com/jtmueller/Collections.Pooled/blob/master/Collections.Pooled/PooledDictionary.cs.

    private const int StartOfFreeList = -3;

    private int[]? buckets;
    private Entry[]? entries;
    // Size of array buckets mut be a prime number, but since the arrays are pooled as powers of two that is never true.
    // So we must to store that prime size here.
    // Accessing indexes past this value is undefined.
    private int size;
    private ulong fastModMultiplier;
    private int count;
    private int freeList;
    private int freeCount;

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => count - freeCount;
    }

    public int EndIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count;
    }

    public void Add(TKey key, TValue value)
    {
        Debug.Assert(key is not null, "key can't be null.");

        int[]? buckets_ = buckets;
        if (buckets_ is null)
        {
            Initialize(0);
            buckets_ = buckets;
            Debug.Assert(buckets_ is not null, "buckets should have been initialized by Initialize method.");
        }

        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null, "entries should have been initialized by Initialize method.");
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);

        uint hashCode = unchecked((uint)key.GetHashCode());
        ref int bucket = ref GetBucket(hashCode, buckets_);

#if DEBUG
        {
            uint collisionCount = 0;
            int i = bucket - 1; // Value in buckets is 1-based.

            ref Entry entry = ref Utils.NullRef<Entry>();

            if (typeof(TKey).IsValueType)
            {
                // Value type: Devirtualize with EqualityComparer<TValue>.Default intrinsic.
                while (i >= 0)
                {
                    Debug.Assert(i < entries_.Length, "Index out of range.");
                    Debug.Assert(i < size, "Accessing index outside prime range.");

                    entry = ref Unsafe.Add(ref entries_Root, i);
                    Debug.Assert(entry.HashCode != hashCode || !EqualityComparer<TKey>.Default.Equals(entry.Key, key), "Key is already found.");

                    i = entry.Next;

                    collisionCount++;
                    // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                    Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
                }
            }
            else
            {
                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                // https://github.com/dotnet/runtime/issues/10050
                // So cache in a local rather than get EqualityComparer per loop iteration
                EqualityComparer<TKey> defaultComparer = EqualityComparer<TKey>.Default;

                // Value type: Devirtualize with EqualityComparer<TValue>.Default intrinsic.
                while (i >= 0)
                {
                    Debug.Assert(i < entries_.Length, "Index out of range.");
                    Debug.Assert(i < size, "Accessing index outside prime range.");

                    entry = ref Unsafe.Add(ref entries_Root, i);
                    Debug.Assert(entry.HashCode != hashCode || !defaultComparer.Equals(entry.Key, key), "Key is already found.");

                    i = entry.Next;

                    collisionCount++;
                    // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                    Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
                }
            }
        }
#endif

        int index;
        int freeCount_ = freeCount;
        if (freeCount_ > 0)
        {
            index = freeList;
            Debug.Assert(index < entries_.Length, "Index out of range.");
            Debug.Assert((StartOfFreeList - entries_[index].Next) >= -1, "Shouldn't overflow because `next` cannot underflow.");
            freeList = StartOfFreeList - Unsafe.Add(ref entries_Root, index).Next;
            freeCount = freeCount_ - 1;
        }
        else
        {
            int count_ = count;
            if (count_ == size)
            {
                Resize();
                buckets_ = buckets;
                Debug.Assert(buckets_ is not null, "buckets should have been initialized by Resize method.");
                entries_ = entries;
                Debug.Assert(entries_ is not null, "entries should have been initialized by Resize method.");
                bucket = ref GetBucket(hashCode, buckets_);
            }
            index = count_;
            count = count_ + 1;
        }

        {
            Debug.Assert(index < entries_.Length, "Index out of range.");
            ref Entry entry = ref Unsafe.Add(ref entries_Root, index);
            entry.HashCode = hashCode;
            entry.Next = bucket - 1; // Value in buckets is 1-based.
            entry.Key = key;
            entry.Value = value;
        }
        bucket = index + 1; // Value in buckets is 1-based.
    }

    public ref TValue GetOrCreateValueSlot(TKey key, out bool found)
    {
        Debug.Assert(key is not null, "key can't be null.");

        int[]? buckets_ = buckets;
        if (buckets_ is null)
        {
            Initialize(0);
            buckets_ = buckets;
            Debug.Assert(buckets_ is not null, "buckets should have been initialized by Initialize method.");
        }

        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null, "entries should have been initialized by Initialize method.");
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);

        uint hashCode = unchecked((uint)key.GetHashCode());
#if DEBUG
        uint collisionCount = 0;
#endif
        ref int bucket = ref GetBucket(hashCode, buckets_);
        int i = bucket - 1; // Value in buckets is 1-based.
        ref Entry entry = ref Utils.NullRef<Entry>();

        if (typeof(TKey).IsValueType)
        {
            // Value type: Devirtualize with EqualityComparer<TValue>.Default intrinsic.
            while (true)
            {
                if (i < 0)
                    break;

                Debug.Assert(i < entries_.Length, "Index out of range.");
                Debug.Assert(i < size, "Accessing index outside prime range.");

                entry = ref Unsafe.Add(ref entries_Root, i);
                if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                    goto ReturnFound;

                i = entry.Next;

#if DEBUG
                collisionCount++;
                // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
#endif
            }
        }
        else
        {
            // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
            // https://github.com/dotnet/runtime/issues/10050
            // So cache in a local rather than get EqualityComparer per loop iteration
            EqualityComparer<TKey> defaultComparer = EqualityComparer<TKey>.Default;

            // Value type: Devirtualize with EqualityComparer<TValue>.Default intrinsic.
            while (true)
            {
                if (i < 0)
                    break;

                Debug.Assert(i < entries_.Length, "Index out of range.");
                Debug.Assert(i < size, "Accessing index outside prime range.");

                entry = ref Unsafe.Add(ref entries_Root, i);
                if (entry.HashCode == hashCode && defaultComparer.Equals(entry.Key, key))
                    goto ReturnFound;

                i = entry.Next;

#if DEBUG
                collisionCount++;
                // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
#endif
            }
        }

        int index;
        int freeCount_ = freeCount;
        if (freeCount_ > 0)
        {
            index = freeList;
            Debug.Assert(index < entries_.Length, "Index out of range.");
            Debug.Assert((StartOfFreeList - entries_[index].Next) >= -1, "Shouldn't overflow because `next` cannot underflow.");
            freeList = StartOfFreeList - Unsafe.Add(ref entries_Root, index).Next;
            freeCount = freeCount_ - 1;
        }
        else
        {
            int count_ = count;
            if (count_ == size)
            {
                Resize();
                buckets_ = buckets;
                Debug.Assert(buckets_ is not null, "buckets should have been initialized by Resize method.");
                entries_ = entries;
                Debug.Assert(entries_ is not null, "entries should have been initialized by Resize method.");
                bucket = ref GetBucket(hashCode, buckets_);
            }
            index = count_;
            count = count_ + 1;
        }

        Debug.Assert(index < entries_.Length, "Index out of range.");
        entry = ref Unsafe.Add(ref entries_Root, index);
        entry.HashCode = hashCode;
        entry.Next = bucket - 1; // Value in buckets is 1-based.
        entry.Key = key;
        bucket = index + 1; // Value in buckets is 1-based.

        found = false;
        goto Return;

    ReturnFound:
        found = true;

    Return:
        return ref entry.Value;
    }

    public void Remove(TKey key)
    {
        Debug.Assert(key is not null, "key can't be null.");

        int[]? buckets_ = buckets;
        if (buckets_ is not null)
        {
            Entry[]? entries_ = entries;
            Debug.Assert(entries_ != null, "Expected entries to not be null.");
            ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);
#if DEBUG
            uint collisionCount = 0;
#endif
            uint hashCode = unchecked((uint)key.GetHashCode());
            ref int bucket = ref GetBucket(hashCode, buckets_);
            int last = -1;
            int i = bucket - 1; // Value in buckets is 1-based
            while (i >= 0)
            {
                Debug.Assert(i < entries_.Length, "Index out of range.");
                ref Entry entry = ref Unsafe.Add(ref entries_Root, i);

                if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    if (last < 0)
                        bucket = entry.Next + 1; // Value in buckets is 1-based
                    else
                    {
                        Debug.Assert(last < entries_.Length, "Index out of range.");
                        Unsafe.Add(ref entries_Root, last).Next = entry.Next;
                    }

                    int freeList_ = freeList;
                    Debug.Assert((StartOfFreeList - freeList_) < 0, "Shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646.");
                    entry.Next = StartOfFreeList - freeList_;

#if !NETSTANDARD2_0
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
#endif
                        entry.Key = default!;

#if !NETSTANDARD2_0
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
#endif
                        entry.Value = default!;

                    freeList = i;
                    freeCount++;
                    return;
                }

                last = i;
                i = entry.Next;

#if DEBUG
                collisionCount++;
                // The chain of entries may forms a loop; which means a concurrent update has happened.
                Debug.Assert(collisionCount <= (uint)entries_.Length, "Concurrent operation error.");
#endif
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        ref TValue valRef = ref FindValue(key);
        if (!Utils.IsNullRef(ref valRef))
        {
            value = valRef;
            return true;
        }

        value = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TValue Get(TKey key)
    {
        ref TValue value = ref FindValue(key);
        Debug.Assert(!Utils.IsNullRef(ref value));
        return value;
    }

    private ref TValue FindValue(TKey key)
    {
        Debug.Assert(key is not null, "key can't be null.");

        ref Entry entry = ref Utils.NullRef<Entry>();
        int[]? buckets_ = buckets;
        if (buckets_ is not null)
        {
            Entry[]? entries_ = entries;
            Debug.Assert(entries_ != null, "Expected entries to not be null.");
            ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);

            uint hashCode = unchecked((uint)key.GetHashCode());
            int i = GetBucket(hashCode, buckets_);
#if DEBUG
            uint collisionCount = 0;
#endif

            if (typeof(TKey).IsValueType)
            {
                // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic.

                i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                while (true)
                {
#if DEBUG
                    // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                    Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
#endif

                    if (i < 0)
                        goto ReturnNotFound;

                    Debug.Assert(i < entries_.Length, "Index out of range.");
                    Debug.Assert(i < size, "Accessing index outside prime range.");

                    entry = ref Unsafe.Add(ref entries_Root, i);
                    if (entry.HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                        goto ReturnFound;

                    i = entry.Next;

#if DEBUG
                    collisionCount++;
#endif
                }
            }
            else
            {
                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                // https://github.com/dotnet/runtime/issues/10050
                // So cache in a local rather than get EqualityComparer per loop iteration
                EqualityComparer<TKey> defaultComparer = EqualityComparer<TKey>.Default;

                i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                while (true)
                {
#if DEBUG
                    // Check if the chain of entries forms a loop; which means a concurrent update has happened.
                    Debug.Assert(collisionCount <= (uint)size, "Concurrent operation error.");
#endif

                    if (i < 0)
                        goto ReturnNotFound;

                    Debug.Assert(i < entries_.Length, "Index out of range.");
                    Debug.Assert(i < size, "Accessing index outside prime range.");

                    entry = ref Unsafe.Add(ref entries_Root, i);
                    if (entry.HashCode == hashCode && defaultComparer.Equals(entry.Key, key))
                        goto ReturnFound;

                    i = entry.Next;

#if DEBUG
                    collisionCount++;
#endif
                }
            }
        }

        goto ReturnNotFound;

    ReturnFound:
        ref TValue value = ref entry.Value;
    Return:
        return ref value;
    ReturnNotFound:
        value = ref Utils.NullRef<TValue>();
        goto Return;
    }

    private void Resize()
    {
        int count_ = count;
        int newSize = HashHelpers.ExpandPrime(count_);

        int[]? buckets_ = buckets;
        Debug.Assert(buckets_ is not null);
        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null);
        Entry[]? oldEntries = entries_;
        bool replaceArrays;

        // Since arrays came from ArrayPool<> and that may give us larger arrays than we asked for,
        // we check if we can use the existing capacity without actually resizing.
        if (buckets_.Length >= newSize && entries_.Length >= newSize)
        {
            Array.Clear(buckets_, 0, buckets_.Length);
            int size_ = size;
            Array.Clear(entries_, size_, newSize - size_);
            replaceArrays = false;
        }
        else
        {
            ArrayUtils.ReturnArray(buckets_);
            buckets_ = ArrayUtils.RentArray<int>(newSize);
            entries_ = ArrayUtils.RentArray<Entry>(newSize);

            Array.Clear(buckets_, 0, buckets_.Length);
            Array.Copy(oldEntries, 0, entries_, 0, count_);
            replaceArrays = true;
        }

        ref int buckets_Root = ref Utils.GetArrayDataReference(buckets_);
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);

        if (IntPtr.Size == sizeof(long))
        {
            ulong fastModMultiplier_ = fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
            for (int i = 0; i < count_; i++)
            {
                Debug.Assert(i < entries_.Length, "Index out of range.");
                ref Entry entry = ref Unsafe.Add(ref entries_Root, i);
                if (entries_[i].Next >= -1)
                {
                    int j = (int)HashHelpers.FastMod(entry.HashCode, (uint)size, fastModMultiplier_);
                    Debug.Assert(j < buckets_.Length, "Index out of range.");
                    ref int bucket = ref Unsafe.Add(ref buckets_Root, j);
                    entry.Next = bucket - 1; // Value in buckets is 1-based.
                    bucket = i + 1;
                }
            }
        }
        else
        {
            for (int i = 0; i < count_; i++)
            {
                Debug.Assert(i < entries_.Length, "Index out of range.");
                ref Entry entry = ref Unsafe.Add(ref entries_Root, i);
                if (entry.Next >= -1)
                {
                    int j = (int)(entry.HashCode % (uint)size);
                    Debug.Assert(j < buckets_.Length, "Index out of range.");
                    ref int bucket = ref Unsafe.Add(ref buckets_Root, j);
                    entry.Next = bucket - 1; // Value in buckets is 1-based.
                    bucket = i + 1;
                }
            }
        }

        if (replaceArrays)
        {
            buckets = buckets_;
            entries = entries_;
            ArrayUtils.ReturnArray(oldEntries, oldEntries.Length);
        }

        size = newSize;
    }

    public void TryShrink()
    {
        Entry[]? oldEntries = entries;
        if (oldEntries is not null)
        {
            int oldCount = count;
            int newSize = HashHelpers.GetPrime(oldCount);

            int currentCapacity = oldEntries.Length;
            if ((newSize / currentCapacity) < ArrayUtils.SHRINK_FACTOR_THRESHOLD && currentCapacity > ArrayUtils.INITIAL_CAPACITY)
            {
                int[]? oldBuckets = buckets;

                Debug.Assert(oldBuckets is not null);

                ArrayUtils.ReturnArray(oldBuckets);

                int[] newBuckets = ArrayUtils.RentArray<int>(newSize);
                Entry[] newEntries = ArrayUtils.RentArray<Entry>(newSize);
                buckets = newBuckets;
                entries = newEntries;

                ref int newBucketsRoot = ref Utils.GetArrayDataReference(newBuckets);
                ref Entry oldEntriesRoot = ref Utils.GetArrayDataReference(oldEntries);
                ref Entry newEntriesRoot = ref Utils.GetArrayDataReference(newEntries);

                int newCount = 0;
                if (IntPtr.Size == sizeof(long))
                {
                    ulong fastModMultiplier_ = fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
                    for (int i = 0; i < oldCount; i++)
                    {
                        Debug.Assert(i < oldEntries.Length, "Index out of range.");
                        if (Unsafe.Add(ref oldEntriesRoot, i).Next >= -1)
                        {
                            Debug.Assert(newCount < newEntries.Length, "Index out of range.");
                            ref Entry entry = ref Unsafe.Add(ref newEntriesRoot, newCount);
                            Debug.Assert(i < newEntries.Length, "Index out of range.");
                            entry = Unsafe.Add(ref newEntriesRoot, i);
                            int j = (int)HashHelpers.FastMod(entry.HashCode, (uint)newSize, fastModMultiplier_);
                            Debug.Assert(j < newBuckets.Length, "Index out of range.");
                            ref int bucket = ref Unsafe.Add(ref newBucketsRoot, j);
                            entry.Next = bucket - 1; // Value in _buckets is 1-based.
                            bucket = newCount + 1;
                            newCount++;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < oldCount; i++)
                    {
                        if (oldEntries[i].Next >= -1)
                        {
                            Debug.Assert(newCount < newEntries.Length, "Index out of range.");
                            ref Entry entry = ref Unsafe.Add(ref newEntriesRoot, newCount);
                            Debug.Assert(i < newEntries.Length, "Index out of range.");
                            entry = Unsafe.Add(ref newEntriesRoot, i);
                            int j = (int)(entry.HashCode % (uint)size);
                            Debug.Assert(j < newBuckets.Length, "Index out of range.");
                            ref int bucket = ref Unsafe.Add(ref newBucketsRoot, j);
                            entry.Next = bucket - 1; // Value in _buckets is 1-based.
                            bucket = newCount + 1;
                            newCount++;
                        }
                    }
                }
                count = newCount;
                size = newSize;
                freeCount = 0;
                ArrayUtils.ReturnArray(oldEntries, oldCount);
            }
        }
    }

    public bool TryGetFromIndex(int index, [NotNullWhen(true)] out TValue value)
    {
        Debug.Assert(index < count, "Index out of range.");

        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null);
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);
        Debug.Assert(index < entries_.Length, "Index out of range.");
        ref Entry entry = ref Unsafe.Add(ref entries_Root, index++);

        if (entry.Next >= -1)
        {
            value = entry.Value!;
            return true;
        }

#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out value);
#else
        value = default;
#endif
        return false;
    }

    public bool MoveNext(ref int index, out KeyValuePair<TKey, TValue> item)
    {
        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null, "Check if Count > 0 property before using this method.");
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);
        while (index < count)
        {
            Debug.Assert(index < entries_.Length, "Index out of range.");
            ref Entry entry = ref Unsafe.Add(ref entries_Root, index++);

            if (entry.Next >= -1)
            {
                item = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
                return true;
            }
        }
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out item);
#else
        item = default;
#endif
        return false;
    }

    public bool MoveNext(ref int index, [NotNullWhen(true)] out TValue value)
    {
        Entry[]? entries_ = entries;
        Debug.Assert(entries_ is not null, "Check if Count > 0 property before using this method.");
        ref Entry entries_Root = ref Utils.GetArrayDataReference(entries_);
        while (index < count)
        {
            Debug.Assert(index < entries_.Length, "Index out of range.");
            ref Entry entry = ref Unsafe.Add(ref entries_Root, index++);

            if (entry.Next >= -1)
            {
                value = entry.Value!;
                return true;
            }
        }
#if NET5_0_OR_GREATER
        Unsafe.SkipInit(out value);
#else
        value = default;
#endif
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        int[]? buckets_ = buckets;
        if (buckets_ is not null)
        {
            ArrayUtils.ReturnArray(buckets_);
            Debug.Assert(entries is not null);
            ArrayUtils.ReturnArray(entries, count);
        }
        this = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref int GetBucket(uint hashCode, int[] buckets)
    {
#if NET5_0_OR_GREATER
        ref int bucketsRoot = ref MemoryMarshal.GetArrayDataReference(buckets);
        int i = (int)(IntPtr.Size == sizeof(long) ? HashHelpers.FastMod(hashCode, (uint)size, fastModMultiplier) : hashCode % (uint)size);
        Debug.Assert(i < buckets.Length, "Index out of range.");
        return ref Unsafe.Add(ref bucketsRoot, i);
#else
        if (IntPtr.Size == sizeof(long))
            return ref buckets[HashHelpers.FastMod(hashCode, (uint)size, fastModMultiplier)];
        return ref buckets[hashCode % (uint)size];
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Initialize(int capacity)
    {
        int size_ = size = HashHelpers.GetPrime(capacity);
        int[] buckets = ArrayUtils.RentArray<int>(size_);
        Entry[] entries = ArrayUtils.RentArray<Entry>(size_);

        // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
        freeList = -1;
        if (IntPtr.Size == sizeof(long))
            fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size_);
        this.buckets = buckets;
        this.entries = entries;
    }

    private struct Entry
    {
        public uint HashCode;
        // 0-based index of next entry in chain.
        // -1 means end of chain.
        // Free list index is enconded by changing sign and subtracting StartOfFreeList.
        public int Next;
        public TKey Key;
        public TValue Value;
    }
}