using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Enderlook.EventManager;

[Generator]
public sealed class EventManagerStrongGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("EventManager.OnceStrong", SourceText.From(GetFile("Once", "the next time", "Once"), Encoding.UTF8));
        context.AddSource("EventManager.MultipleStrong", SourceText.From(GetFile("", "when", "Multiple"), Encoding.UTF8));
    }

    private string GetFile(string methodPostfix, string methodDescription, string typePrefix) =>
$@"
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager;

public sealed partial class EventManager
{{
    {EventManagerGeneratorHelper.GetStrongSubscribeSummary(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe{methodPostfix}<TEvent>(Action<TEvent> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, InvariantObject>(new(callback));
    }}

    {EventManagerGeneratorHelper.GetStrongSubscribeSummary(methodDescription)}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe{methodPostfix}<TEvent>(Action callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, InvariantObject>(new(callback));
    }}

    {EventManagerGeneratorHelper.GetStrongSubscribeSummary(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        if (typeof(TClosure).IsValueType)
            Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, InvariantObjectAndT<TClosure?>>(new(callback, closure));
        else
            Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndT<object?>>(new(callback, closure));
    }}

    {EventManagerGeneratorHelper.GetStrongSubscribeSummary(methodDescription)}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameter}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        if (typeof(TClosure).IsValueType)
            Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, InvariantObjectAndT<TClosure?>>(new(callback, closure));
        else
            Subscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, InvariantObjectAndT<object?>>(new(callback, closure));
    }}

    {EventManagerGeneratorHelper.GetStrongUnsubscribeSummary($"Subscribe{methodPostfix}{{TEvent}}(Action{{TEvent}})")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe{methodPostfix}<TEvent>(Action<TEvent> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObject, StrongActionArgument<TEvent>>, InvariantObjectComparer<Action<TEvent>>, InvariantObject>(new(new(callback)));
    }}

    {EventManagerGeneratorHelper.GetStrongUnsubscribeSummary($"Subscribe{methodPostfix}{{TEvent}}(Action)")}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe{methodPostfix}<TEvent>(Action callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObject, StrongActionVoid<TEvent>>, InvariantObjectComparer<Action>, InvariantObject>(new(new(callback)));
    }}

    {EventManagerGeneratorHelper.GetStrongUnsubscribeSummary($"Subscribe{methodPostfix}{{TEvent, TClosure}}(TClosure, Action{{TClosure, TEvent}})")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        if (typeof(TClosure).IsValueType)
            Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTArgument<TClosure, TEvent>>, StrongValueClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)));
        else
            Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTArgument<object?, TEvent>>, StrongReferenceClosureActionComparer<TClosure?, TEvent>, InvariantObjectAndT<object?>>(new(new(callback, closure)));
    }}

    {EventManagerGeneratorHelper.GetStrongUnsubscribeSummary($"Subscribe{methodPostfix}{{TEvent, TClosure}}(TClosure, Action{{TClosure}})")}
    {EventManagerGeneratorHelper.ClosureParameter}
    {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
    {EventManagerGeneratorHelper.StrongExceptions}
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unsubscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback)
    {{
        if (callback is null)
            ThrowNullCallbackException();

        if (typeof(TClosure).IsValueType)
            Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<TClosure?>, StrongInvariantObjectAndTVoid<TClosure, TEvent>>, StrongValueClosureActionComparer<TClosure?>, InvariantObjectAndT<TClosure?>>(new(new(callback, closure)));
        else
            Unsubscribe<TEvent, Strong{typePrefix}CallbackExecuter<TEvent, InvariantObjectAndT<object?>, StrongInvariantObjectAndTVoid<object?, TEvent>>, StrongReferenceClosureActionComparer<TClosure?>, InvariantObjectAndT<object?>>(new(new(callback, closure)));
    }}
}}
";

    public void Initialize(GeneratorInitializationContext context) { }
}
