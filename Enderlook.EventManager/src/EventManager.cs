using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    /// <summary>
    /// Represent a type safe event manager where types are used as events types.
    /// </summary>
    public sealed partial class EventManager : IDisposable
    {
        // Key type is IInvoker<TEvent>.
        // Value type is actually InvokersHolder<TEvent, TInvoke>.
        private Dictionary<Type, InvokersHolder> holdersPerType = new();

        // Key type is TEvent.
        // Value type is InvokersHolderManager<TEvent>.
        private Dictionary<Type, InvokersHolderManager> managersPerType = new();

        // Elements type derives from InvokersHolder.
        private object[]? holders;
        private int holdersCount;

        /// <summary>
        /// Automatically disposes the object in case it wasn't disposed by the user.
        /// </summary>
        ~EventManager() => Dispose();

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <param name="argument">Arguments of this event.</param>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Raise<TEvent>(TEvent argument)
        {
            ReadBegin();
            {
                if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? manager))
                {
                    FromReadToInHolder();
                    InvokersHolderManager<TEvent> manager_ = Utils.ExpectExactType<InvokersHolderManager<TEvent>>(manager);
                    manager_.Raise(argument, this);
                }
                else
                    ReadEnd();
            }
        }

        /// <summary>
        /// Raises an event type <typeparamref name="TEvent"/> using the parameterless constructor of the type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event.</typeparam>
        /// <exception cref="ObjectDisposedException">Thrown when this instance has already been disposed.</exception>
        public void Raise<TEvent>() where TEvent : new()
            => Raise(new TEvent());

        internal void Unsubscribe<TEvent, TCallbackHelper, TPredicator, TCallback>(TPredicator predicator)
            where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
            where TPredicator : IPredicator<TCallback>
        {
            ReadBegin();
            {
                if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
                {
                    FromReadToInHolder();
                    {
                        Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder)
                            .Unsubscribe(predicator);
                    }
                    InHolderEnd();
                }
                else
                    ReadEnd();
            }
        }

        internal void Subscribe<TEvent, TCallbackHelper, TCallback>(TCallback callback)
            where TCallbackHelper : struct, ICallbackExecuter<TEvent, TCallback>
        {
            ReadBegin();
            {
                if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
                {
                    FromReadToInHolder();
                    {
                        Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder).Subscribe(callback);
                    }
                    InHolderEnd();
                }
                else
                    SlowPath();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            void SlowPath()
            {
                InvokersHolder<TEvent, TCallbackHelper, TCallback> holder_;
                FromReadToWrite();
                {
                    if (holdersPerType.TryGetValue(typeof(TCallbackHelper), out InvokersHolder? holder))
                    {
                        holder_ = Utils.ExpectExactType<InvokersHolder<TEvent, TCallbackHelper, TCallback>>(holder);
                        goto exit;
                    }
                    else
                    {
                        holder_ = new();
                        holdersPerType.Add(typeof(TCallbackHelper), holder_);

                        if (holders is null)
                        {
                            holders = ArrayUtils.RentArray<object>(1);
                            AutoPurger _ = new(this);
                        }

                        ArrayUtils.Add(ref holders, ref holdersCount, holder_);

                        if (managersPerType.TryGetValue(typeof(TEvent), out InvokersHolderManager? packHolder))
                            Utils.ExpectExactType<InvokersHolderManager<TEvent>>(packHolder).Add(holder_);
                        else
                        {
                            InvokersHolderManager<TEvent> packHolder_ = new();
                            packHolder_.Add(holder_);
                            managersPerType.Add(typeof(TEvent), packHolder_);
                        }
                    }
                }
                exit:
                FromWriteToInHolder();
                {
                    holder_.Subscribe(callback);
                }
                InHolderEnd();
            }
        }

        [DoesNotReturn]
        private static void ThrowNullCallbackException() => throw new ArgumentNullException("callback");

        [DoesNotReturn]
        private static void ThrowNullHandleException() => throw new ArgumentNullException("handle");

        [DoesNotReturn]
        private static void ThrowObjectDisposedException() => throw new ObjectDisposedException("Event Manager");
    }
}
