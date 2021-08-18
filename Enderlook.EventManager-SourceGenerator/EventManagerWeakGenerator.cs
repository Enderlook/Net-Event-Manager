using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Enderlook.EventManager
{
    [Generator]
    public sealed class EventManagerWeakGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("EventManager.OnceWeak", SourceText.From(GetFile("once", "Once", "the next time"), Encoding.UTF8));
            context.AddSource("EventManager.MultipleWeak", SourceText.From(GetFile("multiple", "", "when"), Encoding.UTF8));
        }

        private string GetFile(string fieldPrefix, string methodPostfix, string methodDescription) =>
$@"
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enderlook.EventManager
{{
    public sealed partial class EventManager
    {{
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithValueClosureHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithValueClosureWithHandleHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithReferenceClosureWithHandleHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithHandleHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithHandleHandle;

        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakHandleTrackResurrection;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithArgumentWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithValueClosureHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithReferenceClosureHandleTrackResurrection;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}WeakWithValueClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithReferenceClosureWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithArgumentWithHandleHandleTrackResurrection;
        private Dictionary<Type, EventHandle>? {fieldPrefix}WeakWithHandleHandleTrackResurrection;

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            GetOrCreate<Type, MultipleWeakWithArgumentEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            GetOrCreate<Type, MultipleWeakEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakHandleTrackResurrection : ref {fieldPrefix}WeakHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure?>, TEvent>(
                    ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithValueClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object?>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object?, TEvent>>(callback), trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureEventHandle<TEvent, TClosure?>, TEvent>(
                    ref trackResurrection ? ref {fieldPrefix}WeakWithValueClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureEventHandle<TEvent, object?>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithReferenceClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithReferenceClosureHandle,
                typeof(TEvent))
                .Add(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object?>>(callback), trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure?>, TEvent>(
                    ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object?>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object, object?, TEvent>>(callback), trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure?>, TEvent>(
                    ref trackResurrection ? ref {fieldPrefix}WeakWithValueClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)))
                .Add(handle, closure, callback, trackResurrection);
            else
                GetOrCreate<Type, MultipleWeakWithClosureWithHandleEventHandle<TEvent, object?>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithReferenceClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithReferenceClosureWithHandleHandle,
                typeof(TEvent))
                .Add(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object, object?>>(callback), trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            GetOrCreate<Type, MultipleWeakWithArgumentWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithHandleHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakSubscribeSummaryAndHandleParameter(methodDescription)}
        {EventManagerGeneratorHelper.CallbackParameter}
        {EventManagerGeneratorHelper.TrackResurrectionParameter}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakSubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            GetOrCreate<Type, MultipleWeakWithHandleEventHandle<TEvent>, TEvent>(
                ref trackResurrection ? ref {fieldPrefix}WeakWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithHandleHandle,
                typeof(TEvent))
                .Add(handle, callback, trackResurrection);
            InEventEnd();
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{TEvent}}, bool)")}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentHandle,
                typeof(TEvent), out MultipleWeakWithArgumentEventHandle<TEvent>? manager))
            {{
                manager.Remove(handle, callback);
                InEventEnd();
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action, bool)")}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakHandleTrackResurrection : ref {fieldPrefix}WeakHandle,
                typeof(TEvent), out MultipleWeakEventHandle<TEvent>? manager))
            {{
                manager.Remove(handle, callback);
                InEventEnd();
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{TClosure, TEvent}}, bool)")}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithValueClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithReferenceClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithReferenceClosureHandle,
                    typeof(TEvent), out MultipleWeakWithArgumentWithClosureEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object?, TEvent>>(callback));
                    InEventEnd();
                }}
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{TClosure}}, bool)")}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<TClosure?> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithValueClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithValueClosureHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithReferenceClosureHandleTrackResurrection : ref {fieldPrefix}WeakWithReferenceClosureHandle,
                    typeof(TEvent), out MultipleWeakWithClosureEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object?>>(callback));
                    InEventEnd();
                }}
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{THandle, TClosure, TEvent}}, bool)")}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithReferenceClosureWithHandleHandle,
                    typeof(TEvent), out MultipleWeakWithArgumentWithClosureWithHandleEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object, object?, TEvent>>(callback));
                    InEventEnd();
                }}
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TClosure, TEvent}}(THandle, TClosure, Action{{THandle, TClosure}}, bool)")}
        {EventManagerGeneratorHelper.ClosureParameter}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TClosure, TEvent>(THandle handle, TClosure? closure, Action<THandle, TClosure?> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithValueClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithValueClosureWithHandleHandle,
                    new(typeof(TEvent), typeof(TClosure)), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(handle, closure, callback);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithReferenceClosureWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithReferenceClosureWithHandleHandle,
                    typeof(TEvent), out MultipleWeakWithClosureWithHandleEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(handle, CastUtils.AsObject(closure), Unsafe.As<Action<object, object?>>(callback));
                    InEventEnd();
                }}
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{THandle, TEvent}}, bool)")}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithArgumentWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithArgumentWithHandleHandle,
                typeof(TEvent), out MultipleWeakWithArgumentWithHandleEventHandle<TEvent>? manager))
            {{
                manager.Remove(handle, callback);
                InEventEnd();
            }}
        }}

        {EventManagerGeneratorHelper.GetWeakUnsubscribeSummary($"WeakSubscribe{methodPostfix}{{THandle, TEvent}}(THandle, Action{{THandle}}, bool)")}
        {EventManagerGeneratorHelper.CallbackParameterUnsubscribe}
        {EventManagerGeneratorHelper.TrackResurrectionParameterUnsubscribe}
        {EventManagerGeneratorHelper.WeakExceptions}
        public void WeakUnsubscribe{methodPostfix}<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection)
            where THandle : class
        {{
            if (callback is null)
                ThrowNullCallbackException();
            if (handle is null)
                ThrowNullHandleException();

            if (TryGet(ref trackResurrection ? ref {fieldPrefix}WeakWithHandleHandleTrackResurrection : ref {fieldPrefix}WeakWithHandleHandle,
                typeof(TEvent), out MultipleWeakWithHandleEventHandle<TEvent>? manager))
            {{
                manager.Remove(handle, callback);
                InEventEnd();
            }}
        }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
