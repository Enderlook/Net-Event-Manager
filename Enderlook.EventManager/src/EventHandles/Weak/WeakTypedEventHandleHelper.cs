using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal static class WeakTypedEventHandleHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Event<TEvent>(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<EquatableDelegate> @delegate = array[i];
                if (@delegate.TryGetHandle(out object _))
                    CastUtils.ExpectExactType<Action<TEvent>>(@delegate.callback.callback)(argument);
            }

            ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_(Slice slice)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<EquatableDelegate> @delegate = array[i];
                if (@delegate.TryGetHandle(out object _))
                    CastUtils.ExpectExactType<Action>(@delegate.callback.callback)();
            }

            ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_ClosureEvent<TClosure, TEvent>(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                if (@delegate.TryGetHandle(out object _))
                    Unsafe.As<Action<TClosure, TEvent>>(@delegate.callback.callback)(@delegate.callback.closure, argument);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Closure<TClosure>(Slice slice)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                if (@delegate.TryGetHandle(out object _))
                    Unsafe.As<Action<TClosure>>(@delegate.callback.callback)(@delegate.callback.closure);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_HandleClosureEvent<TClosure, TEvent>(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                if (@delegate.TryGetHandle(out object? handle))
                    Unsafe.As<Action<object, TClosure, TEvent>>(@delegate.callback.callback)(handle, @delegate.callback.closure, argument);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_ClosureHandle<TClosure>(Slice slice)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                if (@delegate.TryGetHandle(out object? handle))
                    Unsafe.As<Action<object, TClosure>>(@delegate.callback.callback)(handle, @delegate.callback.closure);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_HandleEvent<TEvent>(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<EquatableDelegate> @delegate = array[i];
                if (@delegate.TryGetHandle(out object? handle))
                    Unsafe.As<Action<object, TEvent>>(@delegate.callback.callback)(handle, argument);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Handle(Slice slice)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                WeakDelegate<EquatableDelegate> @delegate = array[i];
                if (@delegate.TryGetHandle(out object? handle))
                    Unsafe.As<Action<object>>(@delegate.callback.callback)(handle);
            }

            ValueList<WeakDelegate<DelegateWithClosure<EquatableDelegate>>>.Return(slice);
        }
    }
}