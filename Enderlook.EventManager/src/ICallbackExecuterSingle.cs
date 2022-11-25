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
        => Utils.ExecuteActionLike(callback.Unwrap(), argument);
}

internal struct StrongActionVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObject>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObject callback)
        => Utils.ExecuteActionLike(callback.Unwrap());
}

internal struct StrongInvariantObjectAndTArgument<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure?>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure?> callback)
        => Utils.ExecuteActionLike(callback.Value, callback.ValueT, argument);
}

internal struct StrongInvariantObjectAndTVoid<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndT<TClosure?>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndT<TClosure?> callback)
        => Utils.ExecuteActionLike(callback.Value, callback.ValueT);
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
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, argument);
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
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value);
    }
}

internal struct WeakActionHandleVoid<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, tuple.Target);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target);
    }
}

internal struct WeakActionHandleArgument<TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandle>, ICallbackExecuterSingle<TEvent, InvariantObjectAndGCHandleTrackResurrection>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandle callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, tuple.Target, argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndGCHandleTrackResurrection callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, argument);
    }
}

internal struct WeakInvariantObjectAndTArgument<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, callback.ValueT, argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT, argument);
    }
}

internal struct WeakInvariantObjectAndTVoid<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, callback.ValueT);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, callback.ValueT);
    }
}

internal struct WeakInvariantObjectAndTArgumentWithHandle<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, tuple.Target, callback.ValueT, argument);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT, argument);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT, argument);
    }
}

internal struct WeakInvariantObjectAndTVoidWithHandle<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, tuple.Target, callback.ValueT);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
    }
}

internal struct WeakInvariantObjectAndTArgumentWithHandleWithoutClosure<TClosure, TEvent> : ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandle<TClosure>>, ICallbackExecuterSingle<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure>>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandle<TClosure> callback)
    {
#if NET6_0_OR_GREATER
        (object? Target, object? Dependent) tuple = callback.Token.TargetAndDependent;
        Utils.WeakExecuteActionLike(tuple.Target, tuple.Dependent, tuple.Target, callback.ValueT);
#else
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET7_0_OR_GREATER
    static
#endif
    public void Invoke(TEvent argument, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure> callback)
    {
        object? target = callback.Handle.Target;
        Utils.WeakExecuteActionLike(target, callback.Value, target, callback.ValueT);
    }
}
