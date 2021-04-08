using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager<TEventBase>
    {
        private readonly static Action[] emptyActions = new Action[0];
        private readonly static Delegate[] emptyDelegate = new Delegate[0];
        private const int INITIAL_CAPACITY = 4;
        private const int GROW_FACTOR = 2;

        private struct TypeHandle
        {
            private Action[] actions;
            private int actionsCount;

            private Delegate[] delegates;
            private int delegatesCount;

            public static TypeHandle Create(Action action)
            {
                TypeHandle handle = new TypeHandle()
                {
                    actions = new Action[INITIAL_CAPACITY],
                    actionsCount = 1,
                    delegates = emptyDelegate,
                    delegatesCount = 0,
                };
                handle.actions[0] = action;
                return handle;
            }

            public static TypeHandle Create<T>(Action<T> @delegate)
            {
                TypeHandle handle = new TypeHandle()
                {
                    actions = emptyActions,
                    actionsCount = 0,
                    delegates = new Action<T>[INITIAL_CAPACITY],
                    delegatesCount = 1,
                };
                handle.delegates[0] = @delegate;
                return handle;
            }

            public void Raise<T>(T argument)
            {
                for (int i = 0; i < actionsCount; i++)
                    actions[i]();
                for (int i = 0; i < delegatesCount; i++)
                {
                    Debug.Assert(delegates[i] is Action<T>);
                    Unsafe.As<Action<T>>(delegates[i])(argument);
                }
            }

            public void Suscribe(Action action)
            {
                if (actionsCount == actions.Length)
                {
                    if (actionsCount == 0)
                        actions = new Action[INITIAL_CAPACITY];
                    else
                    {
                        Action[] newArray = new Action[actionsCount * GROW_FACTOR];
                        Array.Copy(actions, newArray, actionsCount);
                    }
                }
                actions[actionsCount++] = action;
            }

            public void Unsuscribe(Action action)
            {
                for (int i = 0; i < actionsCount; i++)
                {
                    if (actions[i] == action)
                    {
                        actionsCount--;
                        if (i < actionsCount)
                            Array.Copy(actions, i + 1, actions, i, actionsCount - i);
                        actions[actionsCount] = null;
                    }
                }
            }

            public void Suscribe<T>(Action<T> @delegate)
            {
                if (delegatesCount == delegates.Length)
                {
                    if (delegatesCount == 0)
                        delegates = new Action<T>[INITIAL_CAPACITY];
                    else
                    {
                        Action<T>[] newArray = new Action<T>[delegatesCount * GROW_FACTOR];
                        Array.Copy(delegates, newArray, delegatesCount);
                    }
                }
                delegates[delegatesCount++] = @delegate;
            }

            public void Unsuscribe(Delegate @delegate)
            {
                for (int i = 0; i < delegatesCount; i++)
                {
                    if (delegates[i] == @delegate)
                    {
                        delegatesCount--;
                        if (i < delegatesCount)
                            Array.Copy(delegates, i + 1, delegates, i, delegatesCount - i);
                        delegates[delegatesCount] = null;
                    }
                }
            }
        }
    }
}