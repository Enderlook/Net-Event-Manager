using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using System.Text;

namespace Enderlook.EventManager
{
    [Generator]
    public sealed class EventManagerStrongGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("EventManager.OnceStrong", SourceText.From(GetFile("once", "Once", "the next time"), Encoding.UTF8));
            context.AddSource("EventManager.MultipleStrong", SourceText.From(GetFile("multiple", "", "when"), Encoding.UTF8));
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
        private Dictionary<Type, EventHandle>? {fieldPrefix}StrongWithArgumentHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}StrongHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}StrongWithArgumentWithValueClosureHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}StrongWithArgumentWithReferenceClosureHandle;
        private Dictionary<Type2, EventHandle>? {fieldPrefix}StrongWithValueClosureHandle;
        private Dictionary<Type, EventHandle>? {fieldPrefix}StrongWithReferenceClosureHandle;

        /// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.
        /// </summary>
        /// <param name=""callback"">Callback to execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Subscribe{methodPostfix}<TEvent>(Action<TEvent> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongWithArgumentEventHandle<TEvent>, TEvent>(
                ref {fieldPrefix}StrongWithArgumentHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }}

        /// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.
        /// </summary>
        /// <param name=""callback"">Callback to execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Subscribe{methodPostfix}<TEvent>(Action callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            GetOrCreate<Type, MultipleStrongEventHandle<TEvent>, TEvent>(ref {fieldPrefix}StrongHandle, typeof(TEvent))
                .Add(callback);
            InEventEnd();
        }}

        /// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.
        /// </summary>
        /// <param name=""closure"">Parameter to pass to <paramref name=""callback""/>.</param>
        /// <param name=""callback"">Callback to execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Subscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure?>, TEvent>(
                    ref {fieldPrefix}StrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object?>, TEvent>(
                    ref {fieldPrefix}StrongWithArgumentWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object?, TEvent>>(callback), CastUtils.AsObject(closure));
            InEventEnd();
        }}

        /// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.
        /// </summary>
        /// <param name=""closure"">Parameter to pass to <paramref name=""callback""/>.</param>
        /// <param name=""callback"">Callback to execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Subscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
                GetOrCreate<Type2, MultipleStrongWithClosureEventHandle<TEvent, TClosure?>, TEvent>(
                    ref {fieldPrefix}StrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)))
                    .Add(callback, closure);
            else
                GetOrCreate<Type, MultipleStrongWithClosureEventHandle<TEvent, object?>, TEvent>(
                    ref {fieldPrefix}StrongWithReferenceClosureHandle, typeof(TEvent))
                    .Add(Unsafe.As<Action<object?>>(callback), CastUtils.AsObject(closure));
            InEventEnd();
        }}

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""Subscribe{methodPostfix}{{TEvent}}(Action{{TEvent}})""/>.
        /// </summary>
        /// <param name=""callback"">Callback to no longer execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe{methodPostfix}<TEvent>(Action<TEvent> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref {fieldPrefix}StrongWithArgumentHandle, typeof(TEvent), out MultipleStrongWithArgumentEventHandle<TEvent>? manager))
            {{
                manager.Remove(callback);
                InEventEnd();
            }}
        }}

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""Subscribe{methodPostfix}{{TEvent}}(Action)""/>.
        /// </summary>
        /// <param name=""callback"">Callback to no longer execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe{methodPostfix}<TEvent>(Action callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (TryGet(ref {fieldPrefix}StrongHandle, typeof(TEvent), out MultipleStrongEventHandle<TEvent>? manager))
            {{
                manager.Remove(callback);
                InEventEnd();
            }}
        }}

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""Subscribe{methodPostfix}{{TEvent, TClosure}}(TClosure, Action{{TClosure, TEvent}})""/>.
        /// </summary>
        /// <param name=""closure"">Parameter to pass to <paramref name=""callback""/>.</param>
        /// <param name=""callback"">Callback to no longer execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?, TEvent> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref {fieldPrefix}StrongWithArgumentWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(callback, closure);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref {fieldPrefix}StrongWithArgumentWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithArgumentWithClosureEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(Unsafe.As<Action<object?, TEvent>>(callback), CastUtils.AsObject(closure));
                    InEventEnd();
                }}
            }}
        }}

        /// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""Subscribe{methodPostfix}{{TEvent, TClosure}}(TClosure, Action{{TClosure}})""/>.
        /// </summary>
        /// <param name=""closure"">Parameter to pass to <paramref name=""callback""/>.</param>
        /// <param name=""callback"">Callback to no longer execute.</param>
        /// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>
        public void Unsubscribe{methodPostfix}<TEvent, TClosure>(TClosure? closure, Action<TClosure?> callback)
        {{
            if (callback is null)
                ThrowNullCallbackException();

            if (typeof(TClosure).IsValueType)
            {{
                if (TryGet(ref {fieldPrefix}StrongWithValueClosureHandle, new(typeof(TEvent), typeof(TClosure)), out MultipleStrongWithClosureEventHandle<TEvent, TClosure?>? manager))
                {{
                    manager.Remove(callback, closure);
                    InEventEnd();
                }}
            }}
            else
            {{
                if (TryGet(ref {fieldPrefix}StrongWithReferenceClosureHandle, typeof(TEvent), out MultipleStrongWithClosureEventHandle<TEvent, object?>? manager))
                {{
                    manager.Remove(Unsafe.As<Action<object?>>(callback), CastUtils.AsObject(closure));
                    InEventEnd();
                }}
            }}
        }}
    }}
}}
";

        public void Initialize(GeneratorInitializationContext context) { }
    }
}
