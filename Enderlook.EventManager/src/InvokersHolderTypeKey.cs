using System;
using System.Collections.Generic;

namespace Enderlook.EventManager;

internal readonly struct InvokersHolderTypeKey
{
    // Type is something assignable to ICallbackExecuter<TEvent, TCallback>.
    public readonly Type Type;

    public readonly bool ListenToAssignableEvents;

    public InvokersHolderTypeKey(Type type, bool listenToAssignableEvents)
    {
        Type = type;
        ListenToAssignableEvents = listenToAssignableEvents;
    }

    public override bool Equals(object? obj)
        => obj is InvokersHolderTypeKey key &&
        EqualityComparer<Type>.Default.Equals(Type, key.Type) &&
        ListenToAssignableEvents == key.ListenToAssignableEvents;

    public override int GetHashCode()
    {
#if NETSTANDARD2_0
        unchecked
        {
            int hashCode = 1816491384;
            hashCode = (hashCode * -1521134295) + EqualityComparer<Type>.Default.GetHashCode(Type);
            hashCode = (hashCode * -1521134295) + ListenToAssignableEvents.GetHashCode();
            return hashCode;
        }
#else
        return HashCode.Combine(Type, ListenToAssignableEvents);
#endif
    }
}