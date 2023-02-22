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
        => Utils.ExecuteAction(callback.Unwrap(), argument);
}

internal struct StrongActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObject callback)
        => Utils.ExecuteAction(callback.Unwrap());
}

internal struct StrongInvariantObjectAndTArgument<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure> callback)
        => Utils.ExecuteAction(callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
}

internal struct StrongInvariantObjectAndTVoid<TClosure, TClosureReal, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure> callback)
        => Utils.ExecuteAction(callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
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
        Utils.WeakExecuteAction(tuple.Target, tuple.Dependent, argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, argument);
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
        Utils.WeakExecuteAction(tuple.Target, tuple.Dependent);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value);
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
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
        Utils.WeakExecuteAction(handle, tuple.Dependent, handle);
#else
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle);
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
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
        Utils.WeakExecuteAction(handle, tuple.Dependent, handle, argument);
#else
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, argument);
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
        Utils.WeakExecuteAction(tuple.Target, tuple.Dependent, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
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
        Utils.WeakExecuteAction(tuple.Target, tuple.Dependent, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteAction(target, callback.Value, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
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
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
        Utils.WeakExecuteAction(handle, tuple.Dependent, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
#else
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction<object, TClosureReal, TEvent>(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT), argument);
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
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
        Utils.WeakExecuteAction(handle, tuple.Dependent, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#else
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
    }
}

internal struct WeakInvariantObjectAndTArgumentWithHandleWithoutClosure<TClosure, TClosureReal, THandle, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(tuple.Target);
        Utils.WeakExecuteAction(handle, tuple.Dependent, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#else
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        THandle handle = Utils.ExpectAssignableTypeOrNull<THandle>(target);
        Utils.WeakExecuteAction(handle, callback.Value, handle, Utils.ExpectAssignableTypeOrNull<TClosure, TClosureReal>(callback.ValueT));
    }
}
