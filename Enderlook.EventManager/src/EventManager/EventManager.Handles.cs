using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    public sealed partial class EventManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Purge<TKey, TKey2>(ref Dictionary<TKey, EventHandle>? dictionary, ref ValueList<TKey2> purgedKeys)
            where TKey : notnull, TKey2
        {
            if (dictionary is null)
                return;

            foreach (KeyValuePair<TKey, EventHandle> kvp in dictionary)
            {
                if (kvp.Value.Purge())
                    purgedKeys.Add(kvp.Key);
            }

            for (int i = 0; i < purgedKeys.Count; i++)
                // TODO: Remove this cast.
                dictionary.Remove(CastUtils.ExpectExactType<TKey2, TKey>(ref purgedKeys.Get(i)));

            purgedKeys.Clear();

            if (dictionary.Count == 0)
                dictionary = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGet<TKey, TEventHandle>(ref Dictionary<TKey, EventHandle>? dictionary, TKey key, [NotNullWhen(true)] out TEventHandle? eventHandle)
            where TKey : notnull
            where TEventHandle : EventHandle
        {
            bool found;
            EventHandle? obj;
            ReadBegin();
            {
                if (isDisposedOrDisposing)
                    ThrowObjectDisposedExceptionAndEndRead();

                if (dictionary is null) // We could remove lazy initialization.
                {
                    eventHandle = default;
                    ReadEnd();
                    return false;
                }

                found = dictionary.TryGetValue(key, out obj);
            }

            if (found)
            {
                FromReadToInEvent();
                Debug.Assert(obj is not null);
                eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj!);
                return true;
            }
            else
            {
                ReadEnd();
                eventHandle = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TEventHandle GetOrCreate<TKey, TEventHandle, TEvent>(ref Dictionary<TKey, EventHandle>? dictionary, TKey key)
            where TKey : notnull
            where TEventHandle : TypedEventHandle<TEvent>, new()
        {
            ReadBegin();
            {
                if (isDisposedOrDisposing)
                    ThrowObjectDisposedExceptionAndEndRead();

                if (dictionary is null) // We could remove lazy initialization.
                    return GetOrCreate_SlowPath_CreateDictionary<TKey, TEventHandle, TEvent>(ref dictionary, key);

                if (dictionary.TryGetValue(key, out EventHandle? obj))
                {
                    FromReadToInEvent();
                    return CastUtils.ExpectExactType<TEventHandle>(obj);
                }
                else
                    return GetOrCreate_SlowPath_CreateManager<TKey, TEventHandle, TEvent>(ref dictionary, key);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private TEventHandle GetOrCreate_SlowPath_CreateManager<TKey, TEventHandle, TEvent>(ref Dictionary<TKey, EventHandle>? dictionary, TKey key)
            where TKey : notnull
            where TEventHandle : TypedEventHandle<TEvent>, new()
        {
            TEventHandle eventHandle;
            FromReadToWrite();
            {
                // During the previous call and this one, the dictioanry could be purged.
                if (dictionary is null) // We could remove lazy initialization.
                {
                    dictionary = new Dictionary<TKey, EventHandle>();
                    goto add;
                }

                if (dictionary.TryGetValue(key, out EventHandle? obj))
                {
                    eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj);
                    goto exit;
                }

                add:
                eventHandle = new();
                dictionary.Add(key, eventHandle);
                AddManager<TEventHandle, TEvent>(eventHandle);
            }
            exit:
            FromWriteToInEvent();
            return eventHandle;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private TEventHandle GetOrCreate_SlowPath_CreateDictionary<TKey, TEventHandle, TEvent>(ref Dictionary<TKey, EventHandle>? dictionary, TKey key)
            where TKey : notnull
            where TEventHandle : TypedEventHandle<TEvent>, new()
        {
            TEventHandle eventHandle;
            FromReadToWrite();
            {
                if (dictionary is null) // We could remove lazy initialization.
                {
                    dictionary = new();
                    eventHandle = new();
                    dictionary.Add(key, eventHandle);
                    AddManager<TEventHandle, TEvent>(eventHandle);
                }
                else
                {
                    if (dictionary.TryGetValue(key, out EventHandle? obj))
                        eventHandle = CastUtils.ExpectExactType<TEventHandle>(obj);
                    else
                    {
                        eventHandle = new();
                        dictionary.Add(key, eventHandle);
                        AddManager<TEventHandle, TEvent>(eventHandle);
                    }
                }
            }
            FromWriteToInEvent();
            return eventHandle;
        }

        private void AddManager<TEventHandle, TEvent>(TEventHandle eventHandle)
            where TEventHandle : TypedEventHandle<TEvent>, new()
        {
            if (managersDictionary is null)
            {
                Dictionary<Type, Manager> dictionary = new();
                TypedManager<TEvent> manager = new();
                dictionary.Add(typeof(TEvent), manager);
                manager.Add(eventHandle);
                managersDictionary = dictionary;
                managersList.InitializeWithAndUnlock(manager);
            }
            else
            {
                if (managersDictionary.TryGetValue(typeof(TEvent), out Manager? key))
                    CastUtils.ExpectExactType<TypedManager<TEvent>>(key).Add(eventHandle);
                else
                {
                    TypedManager<TEvent> manager = new();
                    manager.Add(eventHandle);
                    managersDictionary.Add(typeof(TEvent), manager);
                    managersList.Add(manager);
                }
            }
        }
    }
}