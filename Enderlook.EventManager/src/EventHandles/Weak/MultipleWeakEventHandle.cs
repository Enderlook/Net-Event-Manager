using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal abstract class MultipleWeakEventHandle<TEvent, TElement> : TypedEventHandle<TEvent>
        where TElement : IEquatable<TElement>
    {
        private ValueList<WeakDelegate<TElement>> list = ValueList<WeakDelegate<TElement>>.Create();

        public sealed override bool IsEmpty {
            get {
                Debug.Assert(!list.IsLocked);
                return list.Count > 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Add(object handle, TElement callback, bool trackResurrection)
            => list.ConcurrentAdd(new WeakDelegate<TElement>(new WeakReference<object>(handle, trackResurrection), callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Remove<THandle>(THandle handle, TElement callback)
            where THandle : class
            => WeakDelegate<TElement>.ConcurrentRemove(ref list, handle, callback);

        public sealed override bool Purge()
        {
            Debug.Assert(!list.IsLocked);

            if (list.Count == 0)
                goto empty;

            WeakDelegate<TElement>.FreeExpired(ref list);
            if (list.Count == 0)
                goto empty;

            list.TryShrink();
            return false;

            empty:
            list.Return();
            list = ValueList<WeakDelegate<TElement>>.Create();
            return true;
        }

        public sealed override void Dispose() => list.Return();

        public sealed override SliceWithEventHandle ConcurrentGetRaiser()
        {
            // TODO: stored copies could be reduced by borrowing the array.
            ValueList<WeakDelegate<TElement>> list_ = list.Lock();
            WeakDelegate<TElement>.FreeExpired(ref list_);
            ValueList<WeakDelegate<TElement>> clone = list_.Clone();
            list.Unlock(list_);
            return new(clone.ToSlice(), this);
        }
    }

    internal sealed class MultipleWeakWithArgumentEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<TEvent> callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        CastUtils.ExpectExactType<Action<TEvent>>(@delegate.callback.callback)(argument);
                }
            }
            finally
            {
                ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        CastUtils.ExpectExactType<Action>(@delegate.callback.callback)();
                }
            }
            finally
            {
                ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback, closure), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        Unsafe.As<Action<TClosure, TEvent>>(@delegate.callback.callback)(@delegate.callback.closure, argument);
                }
            }
            finally
            {
                ValueList<WeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithClosureEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback, closure), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        Unsafe.As<Action<TClosure>>(@delegate.callback.callback)(@delegate.callback.closure);
                }
            }
            finally
            {
                ValueList<WeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback, closure), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TClosure, TEvent>>(@delegate.callback.callback)(handle, @delegate.callback.closure, argument);
                }
            }
            finally
            {
                ValueList<WeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure> : MultipleWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback, closure), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<WeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TClosure>>(@delegate.callback.callback)(handle, @delegate.callback.closure);
                }
            }
            finally
            {
                ValueList<WeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithArgumentWithHandleEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TEvent>>(@delegate.callback.callback)(handle, argument);
                }
            }
            finally
            {
                ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class MultipleWeakWithHandleEventHandle<TEvent> : MultipleWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class => Add(handle, new(callback), trackResurrection);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            WeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<WeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            try
            {
                for (int i = 0; i < slice.count; i++)
                {
                    WeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object>>(@delegate.callback.callback)(handle);
                }
            }
            finally
            {
                ValueList<WeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }
}