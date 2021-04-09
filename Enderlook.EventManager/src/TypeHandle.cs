using System;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct TypeHandle
    {
        private static readonly Delegate[] emptyDelegate = new Delegate[0];
        private const int INITIAL_CAPACITY = 4;
        private const int GROW_FACTOR = 2;

        private Delegate[] actions;
        private int actionsCount;

        private Delegate[] delegates;
        private int delegatesCount;

        public static TypeHandle Create() => new TypeHandle()
        {
            actions = emptyDelegate,
            actionsCount = 0,
            delegates = emptyDelegate,
            delegatesCount = 0,
        };

        public void Raise<T>(T argument)
        {
            for (int i = 0; i < actionsCount; i++)
                Unsafe.As<Action>(actions[i])();

            for (int i = 0; i < delegatesCount; i++)
                Unsafe.As<Action<T>>(delegates[i])(argument);

        }

        public void Suscribe(Action action)
            => InnnerSuscribe(ref actions, ref actionsCount, action);

        public void Suscribe(Delegate @delegate)
            => InnnerSuscribe(ref delegates, ref delegatesCount, @delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InnnerSuscribe(ref Delegate[] array, ref int count, Delegate action)
        {
            if (count == array.Length)
            {
                if (count == 0)
                    array = new Delegate[INITIAL_CAPACITY];
                else
                {
                    Delegate[] newArray = new Delegate[count * GROW_FACTOR];
                    Array.Copy(array, newArray, count);
                }
            }
            array[count++] = action;
        }

        public void Unsuscribe(Action action) => InnnerUnsuscribe(actions, ref actionsCount, action);

        public void Unsuscribe(Delegate @delegate) => InnnerUnsuscribe(delegates, ref delegatesCount, @delegate);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InnnerUnsuscribe(Delegate[] array, ref int count, Delegate action)
        {
            for (int i = 0; i < count; i++)
            {
                if (array[i].Equals(action))
                {
                    count--;
                    if (i < count)
                        Array.Copy(array, i + 1, array, i, count - i);
                    array[count] = null;
                }
            }
        }
    }
}