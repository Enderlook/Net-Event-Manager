using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Enderlook.EventManager
{
    internal abstract class OnceWeakEventHandle<TEvent, TElement> : TypedEventHandle<TEvent>
        where TElement : IEquatable<TElement>
    {
        private ValueList<OnceWeakDelegate<TElement>> list = ValueList<OnceWeakDelegate<TElement>>.Create();

        public sealed override bool IsEmpty {
            get {
                Debug.Assert(!list.IsLocked);
                return list.Count > 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Add(GCHandle handle, TElement callback) => list.ConcurrentAdd(new OnceWeakDelegate<TElement>(handle, callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Remove(object handle, TElement callback)
            => OnceWeakDelegate<TElement>.ConcurrentRemove(ref list, handle, callback);

        public sealed override bool Purge()
        {
            Debug.Assert(!list.IsLocked);

            if (list.Count == 0)
                goto empty;

            OnceWeakDelegate<TElement>.FreeExpired(ref list);
            if (list.Count == 0)
                goto empty;

            list.TryShrink();
            return false;

            empty:
            list.Return();
            list = ValueList<OnceWeakDelegate<TElement>>.Create();
            return true;
        }

        public sealed override void Dispose()
        {
            int count = list.Count;
            OnceWeakDelegate<TElement>[] array = list.ArrayUnlocked;

            if (unchecked((uint)count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            for (int i = 0; i < count; i++)
                array[i].Free();

            list.Return();
        }

        public sealed override SliceWithEventHandle ConcurrentGetRaiser()
        {
            ValueList<OnceWeakDelegate<TElement>> list_ = list.Lock();
            list.Unlock(ValueList<OnceWeakDelegate<TElement>>.Create());
            return new(list_.ToSlice(), this);
        }
    }

    internal sealed class OnceWeakWithArgumentEventHandle<TEvent> : OnceWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<TEvent> callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        CastUtils.ExpectExactType<Action<TEvent>>(@delegate.callback.callback)(argument);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakEventHandle<TEvent> : OnceWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action callback)
            where THandle : class => base.Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        CastUtils.ExpectExactType<Action>(@delegate.callback.callback)();
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithArgumentWithClosureEventHandle<TEvent, TClosure> : OnceWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        Unsafe.As<Action<TClosure, TEvent>>(@delegate.callback.callback)(@delegate.callback.closure, argument);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithClosureEventHandle<TEvent, TClosure> : OnceWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object _))
                        Unsafe.As<Action<TClosure>>(@delegate.callback.callback)(@delegate.callback.closure);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure> : OnceWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TClosure, TEvent>>(@delegate.callback.callback)(handle, @delegate.callback.closure, argument);
                    j = ++i;
                    array[i].Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithClosureWithHandleEventHandle<TEvent, TClosure> : OnceWeakEventHandle<TEvent, DelegateWithClosure<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback, closure));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, TClosure closure, Action<THandle, TClosure> callback)
            where THandle : class => Remove(handle, new(callback, closure));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<DelegateWithClosure<TClosure>>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<DelegateWithClosure<TClosure>>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<DelegateWithClosure<TClosure>> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TClosure>>(@delegate.callback.callback)(handle, @delegate.callback.closure);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<DelegateWithClosure<TClosure>>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithArgumentWithHandleEventHandle<TEvent> : OnceWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle, TEvent> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object, TEvent>>(@delegate.callback.callback)(handle, argument);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }

    internal sealed class OnceWeakWithHandleEventHandle<TEvent> : OnceWeakEventHandle<TEvent, EquatableDelegate>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add<THandle>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class => Add(GCHandle.Alloc(handle, trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak), new(callback));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove<THandle>(THandle handle, Action<THandle> callback)
            where THandle : class => Remove(handle, new(callback));

        public override void ConcurrentRaise(Slice slice, TEvent argument)
        {
            OnceWeakDelegate<EquatableDelegate>[] array = CastUtils.ExpectExactType<OnceWeakDelegate<EquatableDelegate>[]>(slice.array);

            if (unchecked((uint)slice.count > (uint)array.Length))
            {
                Debug.Fail("Index out of range.");
                return;
            }

            int j = 0;
            try
            {
                for (int i = 0; i < slice.count;)
                {
                    OnceWeakDelegate<EquatableDelegate> @delegate = array[i];
                    if (@delegate.TryGetHandle(out object? handle))
                        Unsafe.As<Action<object>>(@delegate.callback.callback)(handle);
                    j = ++i;
                    @delegate.Free();
                }
            }
            catch
            {
                for (; j < slice.count; j++)
                    array[j].Free();
                throw;
            }
            finally
            {
                ValueList<OnceWeakDelegate<EquatableDelegate>>.Return(slice);
            }
        }
    }
}