﻿using System;
using System.Collections.Generic;
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

        private bool isDisposedOrDisposing;

        private Dictionary<Type, Manager> managersDictionary;
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
            if (managersDictionary.TryGetValue(typeof(TEvent), out Manager managers))
            {
                FromReadToInEvent();
                CastUtils.ExpectExactType<TypedManager<TEvent>>(managers).Raise(this, argument);
            }
        }

        ~EventManager() => Dispose();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ThrowNullCallback() => throw new ArgumentNullException("callback");

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ThrowObjectDisposedExceptionAndEndGlobalRead()
        {
            ReadEnd();
            throw new ObjectDisposedException("Event Manager");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadBegin()
        {
            while (Interlocked.Exchange(ref locked, 1) != 0) ;
            readers++;
            locked = 0;
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