using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

internal interface IPredicator<TElement>
{
    bool DoesMatch(TElement element);
}

internal readonly struct InvariantObjectComparer<T> : IPredicator<InvariantObject>
{
    private readonly InvariantObject source;
    private readonly EqualityComparer<T> comparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InvariantObjectComparer(InvariantObject source)
    {
        this.source = source;
        comparer = EqualityComparer<T>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObject element)
        => comparer.Equals(Utils.ExpectExactType<T>(source.Value), Utils.ExpectExactType<T>(element.Value));
}

internal readonly struct StrongValueClosureActionComparer<TClosure, TEvent> : IPredicator<InvariantObjectAndT<TClosure>>
{
    private readonly InvariantObjectAndT<TClosure> source;
    private readonly EqualityComparer<Action<TClosure, TEvent>> callbackComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StrongValueClosureActionComparer(InvariantObjectAndT<TClosure> source)
    {
        Debug.Assert(typeof(TClosure).IsValueType);
        this.source = source;
        callbackComparer = EqualityComparer<Action<TClosure, TEvent>>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndT<TClosure> element)
        => callbackComparer.Equals(Utils.ExpectExactType<Action<TClosure, TEvent>>(source.Value), Utils.ExpectExactType<Action<TClosure, TEvent>>(element.Value))
        && EqualityComparer<TClosure>.Default.Equals(source.ValueT, element.ValueT);
}

internal readonly struct StrongReferenceClosureActionComparer<TClosure, TEvent> : IPredicator<InvariantObjectAndT<object?>>
{
    private readonly InvariantObjectAndT<object?> source;
    private readonly EqualityComparer<Action<TClosure, TEvent>> callbackComparer;
    private readonly EqualityComparer<TClosure> closureComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StrongReferenceClosureActionComparer(InvariantObjectAndT<object?> source)
    {
        Debug.Assert(!typeof(TClosure).IsValueType);
        this.source = source;
        callbackComparer = EqualityComparer<Action<TClosure, TEvent>>.Default;
        closureComparer = EqualityComparer<TClosure>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndT<object?> element)
        => callbackComparer.Equals(Utils.ExpectExactType<Action<TClosure, TEvent>>(source.Value), Utils.ExpectExactType<Action<TClosure, TEvent>>(element.Value))
        && closureComparer.Equals(Utils.ExpectExactTypeOrNull<TClosure>(source.ValueT), Utils.ExpectExactTypeOrNull<TClosure>(element.ValueT));
}

internal readonly struct StrongValueClosureActionComparer<TClosure> : IPredicator<InvariantObjectAndT<TClosure>>
{
    private readonly InvariantObjectAndT<TClosure> source;
    private readonly EqualityComparer<Action<TClosure>> callbackComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StrongValueClosureActionComparer(InvariantObjectAndT<TClosure> source)
    {
        Debug.Assert(typeof(TClosure).IsValueType);
        this.source = source;
        callbackComparer = EqualityComparer<Action<TClosure>>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndT<TClosure> element)
        => callbackComparer.Equals(Utils.ExpectExactType<Action<TClosure>>(source.Value), Utils.ExpectExactType<Action<TClosure>>(element.Value))
        && EqualityComparer<TClosure>.Default.Equals(source.ValueT, element.ValueT);
}

internal readonly struct StrongReferenceClosureActionComparer<TClosure> : IPredicator<InvariantObjectAndT<object?>>
{
    private readonly InvariantObjectAndT<object?> source;
    private readonly EqualityComparer<Action<TClosure>> callbackComparer;
    private readonly EqualityComparer<TClosure> closureComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StrongReferenceClosureActionComparer(InvariantObjectAndT<object?> source)
    {
        Debug.Assert(!typeof(TClosure).IsValueType);
        callbackComparer = EqualityComparer<Action<TClosure>>.Default;
        closureComparer = EqualityComparer<TClosure>.Default;
        this.source = source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndT<object?> element)
        => callbackComparer.Equals(Utils.ExpectExactType<Action<TClosure>>(source.Value), Utils.ExpectExactType<Action<TClosure>>(element.Value))
        && closureComparer.Equals(Utils.ExpectExactTypeOrNull<TClosure>(source.ValueT), Utils.ExpectExactTypeOrNull<TClosure>(element.ValueT));
}

internal readonly struct InvariantObjectAndGCHandleComparer<TDelegate, THandle> : IPredicator<InvariantObjectAndGCHandle>, IPredicator<InvariantObjectAndGCHandleTrackResurrection>
{
    private readonly TDelegate @delegate;
    private readonly THandle handle;
    private readonly EqualityComparer<TDelegate> delegateComparer;
    private readonly EqualityComparer<THandle> handleComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InvariantObjectAndGCHandleComparer(TDelegate @delegate, THandle handle)
    {
        this.@delegate = @delegate;
        this.handle = handle;
        delegateComparer = EqualityComparer<TDelegate>.Default;
        handleComparer = EqualityComparer<THandle>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndGCHandle element)
    {
#if NET6_0
        (object? Target, object? Dependent) tuple = element.Token.TargetAndDependent;
        return delegateComparer.Equals(@delegate, Utils.ExpectExactTypeOrNull<TDelegate>(tuple.Target))
          && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(tuple.Dependent));
#else
        return delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
          && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndGCHandleTrackResurrection element)
        => delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
        && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
}

internal readonly struct InvariantObjectAndValueTAndGCHandleComparer<TDelegate, TValue, THandle> : IPredicator<InvariantObjectAndTAndGCHandle<TValue>>, IPredicator<InvariantObjectAndTAndGCHandleTrackResurrection<TValue>>
{
    private readonly TDelegate @delegate;
    private readonly TValue value;
    private readonly THandle handle;
    private readonly EqualityComparer<TDelegate> delegateComparer;
    private readonly EqualityComparer<THandle> handleComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InvariantObjectAndValueTAndGCHandleComparer(TDelegate @delegate, TValue value, THandle handle)
    {
        Debug.Assert(typeof(TValue).IsValueType);
        this.@delegate = @delegate;
        this.value = value;
        this.handle = handle;
        delegateComparer = EqualityComparer<TDelegate>.Default;
        handleComparer = EqualityComparer<THandle>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndTAndGCHandle<TValue> element)
    {
#if NET6_0
        (object? Target, object? Dependent) tuple = element.Token.TargetAndDependent;
        return delegateComparer.Equals(@delegate, Utils.ExpectExactTypeOrNull<TDelegate>(tuple.Target))
            && EqualityComparer<TValue>.Default.Equals(value, element.ValueT)
            && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(tuple.Dependent));
#else
        return delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
            && EqualityComparer<TValue>.Default.Equals(value, element.ValueT)
            && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndTAndGCHandleTrackResurrection<TValue> element)
        => delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
        && EqualityComparer<TValue>.Default.Equals(value, element.ValueT)
        && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
}

internal readonly struct InvariantObjectAndReferenceTAndGCHandleComparer<TDelegate, TValue, THandle> : IPredicator<InvariantObjectAndTAndGCHandle<object?>>, IPredicator<InvariantObjectAndTAndGCHandleTrackResurrection<object?>>
{
    private readonly TDelegate @delegate;
    private readonly TValue value;
    private readonly THandle handle;
    private readonly EqualityComparer<TDelegate> delegateComparer;
    private readonly EqualityComparer<TValue> closureComparer;
    private readonly EqualityComparer<THandle> handleComparer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InvariantObjectAndReferenceTAndGCHandleComparer(TDelegate @delegate, TValue value, THandle handle)
    {
        Debug.Assert(!typeof(TValue).IsValueType);
        this.@delegate = @delegate;
        this.value = value;
        this.handle = handle;
        delegateComparer = EqualityComparer<TDelegate>.Default;
        closureComparer = EqualityComparer<TValue>.Default;
        handleComparer = EqualityComparer<THandle>.Default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndTAndGCHandle<object?> element)
    {
#if NET6_0
        (object? Target, object? Dependent) tuple = element.Token.TargetAndDependent;
        return delegateComparer.Equals(@delegate, Utils.ExpectExactTypeOrNull<TDelegate>(tuple.Target))
            && closureComparer.Equals(value, Utils.ExpectExactTypeOrNull<TValue>(element.ValueT))
            && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(tuple.Dependent));
#else
        return delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
            && closureComparer.Equals(value, Utils.ExpectExactTypeOrNull<TValue>(element.ValueT))
            && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool DoesMatch(InvariantObjectAndTAndGCHandleTrackResurrection<object?> element)
        => delegateComparer.Equals(@delegate, Utils.ExpectExactType<TDelegate>(element.Value))
        && closureComparer.Equals(value, Utils.ExpectExactTypeOrNull<TValue>(element.ValueT))
        && handleComparer.Equals(handle, Utils.ExpectExactTypeOrNull<THandle>(element.Handle.Target));
}
