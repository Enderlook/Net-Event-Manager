# Changelog

## 

- Remove unnecessary aggressive inlining in `Raise<TEvent>()` method.
- Remove static constructor from `EventManager`.

## v0.4.3

- Fix and improve documentation.
- Fix racing condition in `DynamicRaise` method.
- Improve performance by avoiding some array index checks.
- Improve incremental capabilities of auto purger when canceled.
- Improve auto purger to avoid a small memory leak produced by storing some internal values in a static field.

## v0.4.2

- Add support for trimming.
- Improve internal array pool sharing (reduces memory consumption and allocation) in .NET >= 5.
- Remove dependencies in .NET >= 5.
- Replace `System.Buffers` and `System.Runtime.CompilerServices.Unsafe` for `System.Memory` dependency in .NET < 5.
- Remove finalizer in `EventManager`.

## 0.4.1

- Fix callbacks that listen to derived event types not being raised unless the derived type has at least one subscribed callback.

## 0.4.0

- Increase performance on weak handles.
- Increase constant propagation of certain branches.
- Improve performance by avoiding covariance checks in arrays and reusing the pool of arrays.
- Improve performance by replacing some virtual calls with direct calls or merging multiple virtual calls into a single one.
- Use `DependentHandle` instead of `GCHandle` were possible on .NET 6 builds.
- Add a timer to auto purger in .NET >= 5 taking into account memory pressure.
- Improve auto purger cancellation capability.
- Improve docummentation in `Raise<TEvent>(TEvent)` method.
- Change APIs:
```diff
public sealed partial class EventManager : IDisposable
{
+   /// A shared default instance of the `EventManager`.
+   public static EventManager Shared { get; }

+   /// Raises event of type `argument.GetType()`, or `typeof(TEvent)` if `argument is null`.
+   public static void DynamicRaise<TEvent>(TEvent argument);

+   /// Raises event of type `typeof(TEvent)` with a new instance of `TEvent` using its parameterless constructor.
+   public void Raise<TEvent>() where TEvent : new();

+   /// Unsubscribes all actions.
+   public void Reset();

-    public void Subscribe<TEvent>(Action<TEvent> callback);
+    public void Subscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Subscribe<TEvent>(Action callback);
+    public void Subscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Unsubscribe<TEvent>(Action<TEvent> callback);
+    public void Unsubscribe<TEvent>(Action<TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Unsubscribe<TEvent>(Action callback);
+    public void Unsubscribe<TEvent>(Action callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);

-    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
+    public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);
-    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
+    public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback, SubscribeFlags subscribeAttributes = SubscribeFlags.Default);

-    public void SubscribeOnce<TEvent>(Action<TEvent> callback);
-    public void SubscribeOnce<TEvent>(Action callback);
-    public void UnsubscribeOnce<TEvent>(Action<TEvent> callback);
-    public void UnsubscribeOnce<TEvent>(Action callback);

-    public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
-    public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
-    public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
-    public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);

-    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);

-    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
+    public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);
-    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
+    public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, WeakSubscribeFlags subscribeAttributes = WeakSubscribeFlags.Default);

-    public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);

-    public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
-    public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
-    public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
}

+/// Determines the configuration of delegate to subscribe.
+[Flags]
+public enum SubscribeFlags
+{
+   /// Default configuration of events.
+   Default = 0,
+   /// The callback is automatically unsubscribed from the event manager after its first execution.
+   RaiseOnce = 1 << 1,
+   /// The callback will listen to any event of assignable type.
+   ListenAssignableEvents = 1 << 2,
+}

+/// Determines the configuration of delegate to subscribe.
+[Flags]
+public enum SubscribeFlags
+{
+   /// Default configuration of events.
+   Default = 0,
+   /// The callback is automatically unsubscribed from the event manager after its first execution.
+   RaiseOnce = 1 << 1,
+   /// The callback will listen to any event of assignable type.
+   ListenAssignableEvents = 1 << 2,
+   //Includes tracking the resurrection of the handle.
+   TrackResurrection = 1 << 3,
+}
```

## v0.3.1

- Reduce assembly size.
- Reduce instance of `EventManager` size.
- Reduce the number of required allocations.
- Reduce initial cost by using lazy initialization.

## v0.3

- Fix exceptions when raising events.
- Fix memory leaks of weak subscribers.
- Fix documentation.
- Reduce garbage produced when a subscribed delegate throws.
- Become auto-cleaning abortable and incremental.
- Add internal multithreading on `.Dispose()` method and auto-cleaning.
- Check for math overflow and underflow on debug builds.

## v0.2.1

- Apply nullable analysis on API.
- Fix some null reference error bugs.

## v0.2.0

- Become `EventManager` thread-safe.
- Increase performance in `EventManager` subscribe and unsubscribe methods by replacing `Delegate.Combine` and `Delegate.Remove` with pooled arrays of delegates.
- Add target frameworks for .Net Standard 2.1 and .Net 5.
- Change the following APIs:
```diff
- public sealed partial class EventManager<TEventBase> : IDisposable
+ public sealed partial class EventManager : IDisposable
{
+   /// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
+   public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+   public void Subscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
+   public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+   public void Unsubscribe<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);

+   /// Subscribes an action to run the next time the event `TEvent` is raised.
+   public void SubscribeOnce<TEvent>(Action<TEvent> callback);
+   public void SubscribeOnce<TEvent>(Action callback);
+   public void UnsubscribeOnce<TEvent>(Action<TEvent> callback);
+   public void UnsubscribeOnce<TEvent>(Action callback);

+   /// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
+   public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+   public void SubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);
+   public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure, TEvent> callback);
+   public void UnsubscribeOnce<TClosure, TEvent>(TClosure closure, Action<TClosure> callback);

+   /// Subscribes an action to run when the event `TEvent` is raised.
+   /// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
+   public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);

+   /// Subscribes an action to run when the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
+   /// A weak reference to handle is stored. If the reference gets garbage collected, the listener is automatically removed.
+   public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribe<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);

+   /// Subscribes an action to run the next time the event `TEvent` is raised.
+   public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TEvent>(THandle handle, Action<THandle> callback, bool trackResurrection);

+   /// Subscribes an action to run the next time the event `TEvent` is raised. The `closure` is passed as a parameter to `callback`.
+   public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakSubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<TClosure> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure, TEvent> callback, bool trackResurrection);
+   public void WeakUnsubscribeOnce<THandle, TClosure, TEvent>(THandle handle, TClosure closure, Action<THandle, TClosure> callback, bool trackResurrection);
}
```

## v0.1.0

Initial Release
