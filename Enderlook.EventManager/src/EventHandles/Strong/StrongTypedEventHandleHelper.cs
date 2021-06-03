using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal static class StrongTypedEventHandleHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Event<TEvent>(Slice slice, TEvent argument)
        {
            EquatableDelegate[] array = CastUtils.ExpectExactType<EquatableDelegate[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                    CastUtils.ExpectExactType<Action<TEvent>>(array[i].callback)(argument);
            }
            finally
            {
                ValueList<EquatableDelegate>.Return(slice);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_(Slice slice)
        {
            EquatableDelegate[] array = CastUtils.ExpectExactType<EquatableDelegate[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                    CastUtils.ExpectExactType<Action>(array[i].callback)();
            }
            finally
            {
                ValueList<EquatableDelegate>.Return(slice);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_ClosureEvent<TClosure, TEvent>(Slice slice, TEvent argument)
        {
            DelegateWithClosure<TClosure>[] array = CastUtils.ExpectExactType<DelegateWithClosure<TClosure>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    DelegateWithClosure<TClosure> element = array[i];
                    Unsafe.As<Action<TClosure, TEvent>>(element.callback)(element.closure, argument);
                }
            }
            finally
            {
                ValueList<DelegateWithClosure<TClosure>>.Return(slice);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Closure<TClosure>(Slice slice)
        {
            DelegateWithClosure<TClosure>[] array = CastUtils.ExpectExactType<DelegateWithClosure<TClosure>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    DelegateWithClosure<TClosure> element = array[i];
                    Unsafe.As<Action<TClosure>>(element.callback)(element.closure);
                }
            }
            finally
            {
                ValueList<DelegateWithClosure<TClosure>>.Return(slice);
            }
        }
    }
}