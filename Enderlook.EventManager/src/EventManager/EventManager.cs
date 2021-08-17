using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    public sealed partial class EventManager : IDisposable
    {
        private int locked;
        private uint readers;
        private int inEvents;
        private uint reserved;

        private int stateLock;
        private int state;
        private const int IS_DISPOSED_OR_DISPOSING = 1 << 1;
        private const int IS_PURGING = 1 << 2;
        private const int IS_CANCELATION_REQUESTED = 1 << 3;

        private int purgingIndex;

        private Dictionary<Type, Manager>? managersDictionary;
        private ValueList<Manager> managersList;

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event</typeparam>
        /// <param name="argument">Arguments of this event.</param>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Raise<TEvent>(TEvent argument)
        {
            ReadBegin();
            if (managersDictionary is null)
                ReadEnd();
            else if (managersDictionary.TryGetValue(typeof(TEvent), out Manager? managers))
            {
                Debug.Assert(managers is not null);
                FromReadToInEvent();
                CastUtils.ExpectExactType<TypedManager<TEvent>>(managers!).Raise(this, argument);
            }
        }

        /// <summary>
        /// Creates the event manager.
        /// </summary>
        public EventManager() => new AutoPurger(this);

        /// <summary>
        /// Automatically disposes the object in case it wasn't disposed by the user.
        /// </summary>
        ~EventManager() => Dispose();

        [DoesNotReturn]
        private static void ThrowNullCallbackException() => throw new ArgumentNullException("callback");

        [DoesNotReturn]
        private static void ThrowNullHandleException() => throw new ArgumentNullException("handle");

        [DoesNotReturn]
        private static void ThrowObjectDisposedException() => throw new ObjectDisposedException("Event Manager");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBegin()
        {
            if (state != 0)
                SlowPath();

            while (Interlocked.Exchange(ref locked, 1) != 0) ;

            if (state == IS_DISPOSED_OR_DISPOSING)
                ThrowObjectDisposedException();

            readers++;
            locked = 0;

            [MethodImpl(MethodImplOptions.NoInlining)]
            void SlowPath()
            {
                int state = this.state;

                if (state == IS_DISPOSED_OR_DISPOSING)
                    ThrowObjectDisposedException();

                if ((state & IS_PURGING) != 0)
                {
                    while (Interlocked.Exchange(ref stateLock, 1) != 0) ; ;
                    {
                        state = this.state;

                        if (state == IS_DISPOSED_OR_DISPOSING)
                        {
                            stateLock = 0;
                            ThrowObjectDisposedException();
                        }

                        if ((state & IS_PURGING) != 0)
                            this.state |= IS_CANCELATION_REQUESTED;
                    }
                    stateLock = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadEnd()
        {
            while (Interlocked.Exchange(ref locked, 1) != 0) ;
            readers--;
            locked = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromReadToWrite()
        {
            while (Interlocked.Exchange(ref locked, 1) != 0) ;
            readers--;

            if (readers == 0)
                return;

            reserved++;
            locked = 0;

            while (true)
            {
                while (Interlocked.Exchange(ref locked, 1) != 0) ;
                if (readers > 0)
                {
                    reserved--;
                    locked = 0;
                }
                else
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MassiveWriteBegin()
        {
            while (true)
            {
                while (Interlocked.Exchange(ref locked, 1) != 0) ;
                if (readers + reserved + inEvents > 0)
                    locked = 0;
                else
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteEnd() => locked = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromReadToInEvent()
        {
            while (Interlocked.Exchange(ref locked, 1) != 0) ;
            readers--;
            Interlocked.Increment(ref inEvents);
            locked = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FromWriteToInEvent()
        {
            Interlocked.Increment(ref inEvents);
            locked = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void InEventEnd() => Interlocked.Decrement(ref inEvents);
    }
}