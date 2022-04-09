using System;

namespace Enderlook.EventManager;

/// <summary>
/// Determines the configuration of delegate to subscribe.
/// </summary>
[Flags]
public enum SubscribeFlags
{
    /// <summary>
    /// By default callbacks can be executed multiple times and only listen to the exact type match.<br/>
    /// This behaviour can be overriden by applying other flags of this <see cref="SubscribeFlags"/>.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The callback is automatically unsubscribed from the <see cref="EventManager"/> after its first execution.
    /// </summary>
    RaiseOnce = 1 << 1,

    /// <summary>
    /// The callback will listen to any event of assignable type.<br/>
    /// For example, a delegate <c>Action&lt;BaseEvent&gt;</c> will not only listen for type <c>BaseEvent</c> but also for <c>ConcreteEvent</c> given <c>ConcreteEvent</c> is a subtype of <c>BaseEvent</c>.<br/>
    /// This also supports interface types.<br/>
    /// When the derived type is a value-type and the callback takes a reference-type (i.e: <c>Action&lt;<see langword="object"/>&gt;</c> and derived type is <c><see langword="struct"/> SomeEvent</c>), the boxed event is shared to all delegate calls (and may or may not be pooled).<br/>
    /// By convention, value-types should be immutable, so this is not a problem. However, if you have a mutable type, be warned.
    /// </summary>
    ListenAssignableEvents = 1 << 2,
}
