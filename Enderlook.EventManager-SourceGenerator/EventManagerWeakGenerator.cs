﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Enderlook.EventManager;

[Generator]
public sealed class EventManagerWeakGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("EventManager.OnceWeak", SourceText.From(GetFile("Once", "the next time", "Once"), Encoding.UTF8));
        context.AddSource("EventManager.MultipleWeak", SourceText.From(GetFile("", "when", "Multiple"), Encoding.UTF8));
    }

    private string GetFile(string methodPostfix, string methodDescription, string typePrefix)
    {
        return $@"
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{{
    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionArgument<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, handle, closure));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, handle, closure));
            else
                Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, handle, closure));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.TrackResurrectionParameter}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Subscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{TEvent}}, bool)")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action<TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action, bool)")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionVoid<TEvent>>, InvariantObjectAndGCHandleComparer<Action, THandle>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{TClosure, TEvent}}, bool)")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgument<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgument<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{TClosure}}, bool)")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoid<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{THandle, TClosure, TEvent}}, bool)")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTArgumentWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTArgumentWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?, TEvent>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{THandle, TClosure}}, bool)")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandleTrackResurrection<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandleTrackResurrection<object?>>(new(callback, closure, handle));
        }}
        else
        {{
            if (typeof(TClosure).IsValueType)
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<TClosure?>, WeakInvariantObjectAndTVoidWithHandle<TClosure?, TEvent>>, InvariantObjectAndValueTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<TClosure?>>(new(callback, closure, handle));
            else
                Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndTAndGCHandle<object?>, WeakInvariantObjectAndTVoidWithHandle<object?, TEvent>>, InvariantObjectAndReferenceTAndGCHandleComparer<Action<THandle, TClosure?>, TClosure?, THandle>, InvariantObjectAndTAndGCHandle<object?>>(new(callback, closure, handle));
        }}
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{THandle, TEvent}}, bool)")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleArgument<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle, TEvent>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}

    {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{THandle}}, bool)")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
    {EventManagerGeneratorHelper.WeakExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
        where THandle : class
    {{
        if (callback is null)
            ThrowNullCallbackException();
        if (handle is null)
            ThrowNullHandleException();

        if (trackResurrection)
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandleTrackResurrection, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandleTrackResurrection>(new(callback, handle));
        else
            Unsubscribe<TEvent, Weak{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndGCHandle, WeakActionHandleVoid<THandle, TEvent>>, InvariantObjectAndGCHandleComparer<Action<THandle>, THandle>, InvariantObjectAndGCHandle>(new(callback, handle));
    }}
}}
";
    }

    public void Initialize(GeneratorInitializationContext context) { }
}