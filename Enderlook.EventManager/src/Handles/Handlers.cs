using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal struct Handles<TKey, TValue>
        where TValue : Handle
    {
        private ReadWriterLock @lock;
        private Dictionary<TKey, TValue> dictionary;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Handles<TKey, TValue> Create()
            => new()
        {
            dictionary = new(),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<TList, TDelegate, TMode, TClosure, TEvent>(out EventHandle<TList, TDelegate, TMode, TClosure, TEvent> result)
            where TList : IEventCollection<TDelegate>
        {
            TKey key = GetKey<TMode, TClosure, TEvent>();

            @lock.ReadBegin();
            if (dictionary.TryGetValue(key, out TValue value))
            {
                @lock.ReadEnd();
                Debug.Assert(value is EventHandle<TList, TDelegate, TMode, TClosure, TEvent>);
                result = Unsafe.As<EventHandle<TList, TDelegate, TMode, TClosure, TEvent>>(value);
                return true;
            }
            @lock.ReadEnd();
            result = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static TKey GetKey<TMode, TClosure, TEvent>()
        {
            if (typeof(TMode) == typeof(HasNoClosure))
            {
                Debug.Assert(typeof(TClosure) == typeof(Unused));
                Debug.Assert(typeof(TKey) == typeof(Type));
                Type type = typeof(TEvent);
                return Unsafe.As<Type, TKey>(ref type);
            }
            else if (typeof(TMode) == typeof(HasClosure))
            {
                Debug.Assert(typeof(TClosure) != typeof(Unused));
                if (typeof(TClosure).IsValueType)
                {
                    Debug.Assert(typeof(TKey) == typeof((Type, Type)));
                    (Type, Type) type = (typeof(TEvent), typeof(TClosure));
                    return Unsafe.As<(Type, Type), TKey>(ref type);
                }
                else
                {
                    Debug.Assert(typeof(TKey) == typeof(Type));
                    Type type = typeof(TEvent);
                    return Unsafe.As<Type, TKey>(ref type);
                }
            }

            Debug.Fail("Impossible state.");
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EventHandle<TList, TDelegate, TMode, TClosure, TEvent> GetOrCreate<TList, TDelegate, TMode, TClosure, TEvent>(ref ReadWriterLock globalLock, Dictionary<Type, Handle> handles)
            where TList : IEventCollection<TDelegate>
        {
            TKey key = GetKey<TMode, TClosure, TEvent>();

            @lock.ReadBegin();
            if (dictionary.TryGetValue(key, out TValue value))
            {
                @lock.ReadEnd();
                if (typeof(TClosure).IsValueType)
                    Debug.Assert(value is EventHandle<TList, TDelegate, TMode, TClosure, TEvent>);
                else
                    Debug.Assert(value is EventHandle<TList, TDelegate, TMode, object, TEvent>);
                return Unsafe.As<EventHandle<TList, TDelegate, TMode, TClosure, TEvent>>(value);
            }

            return SlowPath(ref this, ref globalLock, handles, key);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static EventHandle<TList, TDelegate, TMode, TClosure, TEvent> SlowPath(ref Handles<TKey, TValue> self, ref ReadWriterLock globalLock, Dictionary<Type, Handle> handles, TKey key)
            {
                self.@lock.ReadEnd();
                self.@lock.WriteBegin();
                if (self.dictionary.TryGetValue(key, out TValue value))
                {
                    self.@lock.WriteEnd();

                    if (typeof(TClosure).IsValueType)
                        Debug.Assert(value is EventHandle<TList, TDelegate, TMode, TClosure, TEvent>);
                    else
                        Debug.Assert(value is EventHandle<TList, TDelegate, TMode, object, TEvent>);

                    return Unsafe.As<EventHandle<TList, TDelegate, TMode, TClosure, TEvent>>(value);
                }
                else
                {
                    EventHandle<TList, TDelegate, TMode, TClosure, TEvent> eventHandle;

                    if (typeof(TClosure).IsValueType)
                        eventHandle = new();
                    else
                    {
                        EventHandle<TList, TDelegate, TMode, object, TEvent> tmp = new();
                        eventHandle = Unsafe.As<EventHandle<TList, TDelegate, TMode, TClosure, TEvent>>(tmp);
                    }

                    Debug.Assert(eventHandle is TValue);
                    self.dictionary.Add(key, Unsafe.As<EventHandle<TList, TDelegate, TMode, TClosure, TEvent>, TValue>(ref eventHandle));

                    globalLock.ReadBegin();
                    if (handles.TryGetValue(typeof(TEvent), out Handle handle))
                    {
                        globalLock.ReadEnd();
                        Debug.Assert(handle is GlobalHandle<TEvent>);
                        Unsafe.As<GlobalHandle<TEvent>>(handle).Add(eventHandle);
                    }
                    else
                    {
                        globalLock.ReadEnd();
                        globalLock.WriteBegin();
                        if (handles.TryGetValue(typeof(TEvent), out handle))
                        {
                            globalLock.WriteEnd();
                            Debug.Assert(handle is GlobalHandle<TEvent>);
                            Unsafe.As<GlobalHandle<TEvent>>(handle).Add(eventHandle);
                        }
                        else
                        {
                            GlobalHandle<TEvent> globalHandle = new();
                            handles.Add(typeof(TEvent), globalHandle);
                            globalHandle.Add(eventHandle);
                            globalLock.WriteEnd();
                        }
                    }

                    self.@lock.WriteEnd();
                    return eventHandle;
                }
            }
        }
    }
}