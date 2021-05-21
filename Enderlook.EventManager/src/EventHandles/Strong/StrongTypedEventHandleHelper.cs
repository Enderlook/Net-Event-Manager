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
            object[] array = CastUtils.ExpectExactType<object[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
                CastUtils.ExpectExactType<Action<TEvent>>(array[i])(argument);

            ValueList<object>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_(Slice slice)
        {
            object[] array = CastUtils.ExpectExactType<object[]>(slice.array);

            if ((uint)slice.count > (uint)slice.array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
                CastUtils.ExpectExactType<Action>(array[i])();

            ValueList<object>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_ClosureEvent<TClosure, TEvent>(Slice slice, TEvent argument)
        {
            DelegateWithClosure<TClosure>[] array = CastUtils.ExpectExactType<DelegateWithClosure<TClosure>[]>(slice.array);

            if ((uint)slice.count > (uint)array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                DelegateWithClosure<TClosure> element = array[i];
                Unsafe.As<Action<TClosure, TEvent>>(element.callback)(element.closure, argument);
            }

            ValueList<DelegateWithClosure<TClosure>>.Return(slice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConcurrentRaise_Closure<TClosure>(Slice slice)
        {
            DelegateWithClosure<TClosure>[] array = CastUtils.ExpectExactType<DelegateWithClosure<TClosure>[]>(slice.array);

            if ((uint)slice.count > (uint)array.Length)
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < slice.count; i++)
            {
                DelegateWithClosure<TClosure> element = array[i];
                Unsafe.As<Action<TClosure>>(element.callback)(element.closure);
            }

            ValueList<DelegateWithClosure<TClosure>>.Return(slice);
        }
    }
}