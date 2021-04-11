using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    internal static class ConcurrentPool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Rent<T>() => Pool<T>.Rent();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return<T>(T[] array) => Pool<T>.Return(array);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Empty<T>() => Pool<T>.empty;

        private static class Pool<T>
        {
            private const int ARRAY_LENGTH = 100;
            private const int INITIAL_LENGTH = 4;

            public static readonly T[] empty = new T[0];

            private static readonly T[][] pool = new T[ARRAY_LENGTH][];
            private static int count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T[] Rent()
            {
                while (true)
                {
                    int index = Interlocked.Decrement(ref count);
                    if (index < 0)
                        return ArrayPool<T>.Shared.Rent(INITIAL_LENGTH);
                    else
                    {
                        if (index < ARRAY_LENGTH)
                        {
                            T[] obj = Interlocked.Exchange(ref pool[index], null);
                            if (!(obj is null))
                                return obj;
                        }
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void Return(T[] array)
            {
                int index = Interlocked.Increment(ref count);
                if (index < 0)
                    ArrayPool<T>.Shared.Return(array);
                else if (index < ARRAY_LENGTH)
                    pool[index] = array;
                else
                    ArrayPool<T>.Shared.Return(array);
            }
        }
    }
}