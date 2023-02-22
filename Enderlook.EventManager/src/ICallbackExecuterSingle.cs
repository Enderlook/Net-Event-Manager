using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal interface ICallbackExecuterSingle<TEvent, TCallback>
{
#if NET7_0_OR_GREATER
    static abstract
#endif
    void Invoke(TEvent argument, TCallback callback);
}

internal struct StrongActionArgument<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObject callback)
        => Utils.ExpectExactType<Action<TEvent>>(callback.Unwrap())(argument);
}

internal struct StrongActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObject callback)
        => Utils.ExpectExactType<Action>(callback.Unwrap())();
}

internal struct StrongInvariantObjectAndTArgument<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure> callback)
        => Utils.ExpectExactType<Action<TClosureReal, TEvent>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
}

internal struct StrongInvariantObjectAndTVoid<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure> callback)
        => Utils.ExpectExactType<Action<TClosureReal>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
}

internal struct WeakActionArgument<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<TEvent>>(tuple.Dependent)(argument);
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TEvent>>(callback.Value)(argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TEvent>>(callback.Value)(argument);
    }
}

internal struct WeakActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action>(tuple.Dependent)();
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action>(callback.Value)();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action>(callback.Value)();
    }
}

internal struct WeakActionHandleVoid<THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<THandle>>(tuple.Dependent)(handle);
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle>>(callback.Value)(handle);
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle>>(callback.Value)(handle);
        }
    }
}

internal struct WeakActionHandleArgument<THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<THandle, TEvent>>(tuple.Dependent)(handle, argument);
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle, TEvent>>(callback.Value)(handle, argument);
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle, TEvent>>(callback.Value)(handle, argument);
        }
    }
}

internal struct WeakInvariantObjectAndTArgument<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(tuple.Dependent)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
    }
}

internal struct WeakInvariantObjectAndTVoid<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<TClosureReal>>(tuple.Dependent)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TClosureReal>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
            Utils.ExpectExactType<Action<TClosureReal>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
    }
}

internal struct WeakInvariantObjectAndTArgumentWithHandle<TClosure, TClosureReal, THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(tuple.Dependent)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<TClosureReal, TEvent>>(callback.Value)(Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
        }
    }
}

internal struct WeakInvariantObjectAndTVoidWithHandle<TClosure, TClosureReal, THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        if (tuple.Target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
            Debug.Assert(tuple.Dependent is not null);
            Utils.ExpectExactType<Action<THandle, TClosureReal>>(tuple.Dependent)(handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
        }
#else
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle, TClosureReal>>(callback.Value)(handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        if (target is not null)
        {
            THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
            Utils.ExpectExactType<Action<THandle, TClosureReal>>(callback.Value)(handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
        }
    }
}