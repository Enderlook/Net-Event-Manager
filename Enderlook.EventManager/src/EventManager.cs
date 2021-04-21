using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    public sealed partial class EventManager : IDisposable
    {
        private ReadWriterLock @lock;
        private Dictionary<Type, Handle> handles = new Dictionary<Type, Handle>();

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="eventArgument">Arguments of this event</param>
        public void Raise<TEvent>(TEvent eventArgument)
        {
            @lock.ReadBegin();
            if (handles.TryGetValue(typeof(TEvent), out Handle handle))
            {
                @lock.ReadEnd();
                Debug.Assert(handle is GlobalHandle<TEvent>);
                Unsafe.As<GlobalHandle<TEvent>>(handle).Raise(eventArgument);
            }
            else
                @lock.ReadEnd();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            @lock.ReadBegin();
            foreach (Handle handle in handles.Values)
                handle.Dispose();
            @lock.ReadEnd();
        }

        /// <summary>
        /// Force the purge of removed delegates.
        /// </summary>
        public void Purge()
        {
            @lock.ReadBegin();
            foreach (Handle handle in handles.Values)
                handle.CompactAndPurge();
            @lock.ReadEnd();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullHandle() => throw new ArgumentNullException("handle");
    }
}