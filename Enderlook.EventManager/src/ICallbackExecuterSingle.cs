﻿using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{
    internal interface ICallbackExecuterSingle<TEvent, TCallback>
    {
        void Invoke(TEvent argument, TCallback callback);
    }

    internal struct StrongActionArgument<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObject callback)
            => Utils.ExecuteActionLike(callback.Value, argument);
    }

    internal struct StrongActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObject callback)
            => Utils.ExecuteActionLike(callback.Value);
    }

    internal struct StrongInvariantObjectAndTArgument<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure?>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndT<TClosure?> callback)
            => Utils.ExecuteActionLike(callback.Value, callback.ValueT, argument);
    }

    internal struct StrongInvariantObjectAndTVoid<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure?>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndT<TClosure?> callback)
            => Utils.ExecuteActionLike(callback.Value, callback.ValueT);
    }

    internal struct WeakActionArgument<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>,  ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, argument);
        }
    }

    internal struct WeakActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value);
        }
    }

    internal struct WeakActionHandleVoid<THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, Utils.ExpectExactTypeOrNull<THandle>(target));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, Utils.ExpectExactTypeOrNull<THandle>(target));
        }
    }

    internal struct WeakActionHandleArgument<THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, Utils.ExpectExactTypeOrNull<THandle>(target), argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, Utils.ExpectExactTypeOrNull<THandle>(target), argument);
        }
    }

    internal struct WeakInvariantObjectAndTArgument<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT, argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT, argument);
        }
    }

    internal struct WeakInvariantObjectAndTVoid<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT);
        }
    }

    internal struct WeakInvariantObjectAndTArgumentWithHandle<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT, argument);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT, argument);
        }
    }

    internal struct WeakInvariantObjectAndTVoidWithHandle<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
        }
    }

    internal struct WeakInvariantObjectAndTArgumentWithHandleWithoutClosure<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
        {
            object? target = callback.Handle.Target;
            Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
        }
    }
}