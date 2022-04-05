# Changelog

##

- Increase perfomance on weak handles.
- Increase constant propagation of certain branches.
- Improve perfomance by avoiding covariance checks in arrays and reusing pool of arrays.
- Use `DependentHandle` instead of `GCHandle` were possible on .NET 6 builds.
- Add timer to auto purger in .NET >= 5 taking into account memory pressure.
- Improve auto purger cancellation capability.
- Add the following APIs:
```diff
public sealed partial class EventManager : IDisposable
{
+   /// A shared default instance of the EventManager.
+   public static EventManager Shared { get; }

+   /// Raises event using the parameterless constructor of the event type.
+   public void Raise<TEvent>() where TEvent : new();

+   /// Unsubscribes all actions.
+   public void Reset();
}
```

## v0.3.1

- Reduce assembly size.
- Reduce instance of `EventManager` size.
- Reduce amount of required allocations.
- Reduce initial cost by using lazily initialization.

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
- Increase perfomance in `EventManager` subscribe and unsubscribe methods by replacing `Delegate.Combine` and `Delegate.Remove` with pooled arrays of delegates.
- Add target frameworks for .Net Standard 2.1 and .Net 5.
- Modify following APIs:
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
