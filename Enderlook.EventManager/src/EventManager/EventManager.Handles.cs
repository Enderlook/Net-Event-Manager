using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        internal struct Handles<TKey>
        {
            private ReadWriteLock @lock;
            private Dictionary<TKey, EventHandle> dictionary;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Purge(ref ValueList<TKey> purgedKeys)
            {
                ValueList<TKey> keys = purgedKeys;

                foreach (KeyValuePair<TKey, EventHandle> kvp in dictionary)
                {
                    if (kvp.Value.Purge())
                        keys.Add(kvp.Key);
                }

                for (int i = 0; i < keys.Count; i++)
                    dictionary.Remove(keys.Get(i));

                keys.Clear();
                purgedKeys = keys;

                if (dictionary.Count == 0)
                    dictionary = null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGet<TEventHandle>(TKey key, out TEventHandle eventHandle)
                where TEventHandle : EventHandle
            {
                if (dictionary is null) // We could remove lazy initialization.
                {
                    eventHandle = default;
                    return false;
                }

                bool found;
                EventHandle obj;
                @lock.ReadBegin();
                {
                    found = dictionary.TryGetValue(key, out obj);
                }
                @lock.ReadEnd();
                if (found)
                {
                    eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj);
                    return true;
                }
                else
                {
                    eventHandle = default;
                    return false;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TEventHandle GetOrCreate<TEventHandle, TEvent>(TKey key, EventManager eventManager)
                where TEventHandle : TypedEventHandle<TEvent>, new()
            {
                if (dictionary is null) // We could remove lazy initialization.
                    return GetOrCreate_SlowPath_CreateDictionary<TEventHandle, TEvent>(key, eventManager);

                @lock.ReadBegin();
                if (dictionary.TryGetValue(key, out EventHandle obj))
                {
                    @lock.ReadEnd();
                    return CastUtils.ExpectExactType<TEventHandle>(obj);
                }
                else
                    return GetOrCreate_SlowPath_CreateManager<TEventHandle, TEvent>(key, eventManager);
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private TEventHandle GetOrCreate_SlowPath_CreateManager<TEventHandle, TEvent>(TKey key, EventManager eventManager)
                where TEventHandle : TypedEventHandle<TEvent>, new()
            {
                @lock.ReadEnd();
                TEventHandle eventHandle;
                @lock.WriteBegin();
                if (dictionary.TryGetValue(key, out EventHandle obj))
                    eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj);
                else
                {
                    eventHandle = new();
                    dictionary.Add(key, eventHandle);
                    AddManager<TEventHandle, TEvent>(eventManager, eventHandle);
                }
                @lock.WriteEnd();
                return eventHandle;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private TEventHandle GetOrCreate_SlowPath_CreateDictionary<TEventHandle, TEvent>(TKey key, EventManager eventManager)
                where TEventHandle : TypedEventHandle<TEvent>, new()
            {
                TEventHandle eventHandle;
                @lock.WriteBegin();
                {
                    if (dictionary is null)
                    {
                        dictionary = new();
                        eventHandle = new();
                        dictionary.Add(key, eventHandle);
                        AddManager<TEventHandle, TEvent>(eventManager, eventHandle);
                    }
                    else
                    {
                        if (dictionary.TryGetValue(key, out EventHandle obj))
                            eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj);
                        else
                        {
                            eventHandle = new();
                            dictionary.Add(key, eventHandle);
                            AddManager<TEventHandle, TEvent>(eventManager, eventHandle);
                        }
                    }
                }
                @lock.WriteEnd();
                return eventHandle;
            }

            private static void AddManager<TEventHandle, TEvent>(EventManager eventManager, TEventHandle eventHandle)
                where TEventHandle : TypedEventHandle<TEvent>, new()
            {
                int count_ = eventManager.managersList.LockAndGetCount();
                if (eventManager.managersList.IsDefault)
                {
                    Debug.Assert(eventManager.managersDictionary is null);

                    Dictionary<Type, Manager> dictionary = new();
                    TypedManager<TEvent> manager = new();
                    dictionary.Add(typeof(TEvent), manager);
                    manager.Add(eventHandle);
                    eventManager.managersDictionary = dictionary;
                    eventManager.managersList.InitializeWithAndUnlock(manager);
                }
                else
                {
                    Debug.Assert(eventManager.managersDictionary is not null);

                    if (eventManager.managersDictionary.TryGetValue(typeof(TEvent), out Manager key))
                    {
                        CastUtils.ExpectExactType<TypedManager<TEvent>>(key).Add(eventHandle);
                        eventManager.managersList.Unlock(count_);
                    }
                    else
                    {
                        TypedManager<TEvent> manager = new();
                        manager.Add(eventHandle);
                        eventManager.managersDictionary.Add(typeof(TEvent), manager);
                        eventManager.managersList.AddAndUnlock(count_, manager);
                    }
                }
            }
        }
    }
}