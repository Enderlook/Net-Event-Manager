namespace Enderlook.EventManager;

public sealed class EventManagerGeneratorHelper
{
    public const string CallbackParameter =
        @"/// <param name=""callback"">Callback to execute.</param>";

    public const string CallbackParameterUnsubscribe =
        @"/// <param name=""callback"">Callback to no longer execute.</param>";

    public const string ClosureParameter =
        @"/// <param name=""closure"">Parameter to pass to <paramref name=""callback""/>.</param>";

    public const string StrongExceptions =
        @"/// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>";

    public const string TrackResurrectionParameter =
        @"/// <param name=""trackResurrection"">Whenever it should track the resurrection of the handle.</param>";

    public const string TrackResurrectionParameterUnsubscribe =
        @"/// <param name=""trackResurrection"">Whenever it was tracking the resurrection of the handle.</param>";

    public const string WeakExceptions =
        @"/// <exception cref=""ArgumentNullException"">Thrown when <paramref name=""callback""/> or <paramref name=""handle""/> is <see langword=""null""/>.</exception>
        /// <exception cref=""ObjectDisposedException"">Thrown when this instance has already been disposed.</exception>";

    public static string GetStrongSubscribeSummary(string methodDescription) => 
        $@"/// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.
        /// </summary>";

    public static string GetStrongUnsubscribeSummary(string methodReference) =>
        $@"/// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""{methodReference}""/>.
        /// </summary>";

    public static string GetWeakSubscribeSummaryAndHandleParameter(string methodDescription) =>
        $@"/// <summary>
        /// Subscribes the callback <paramref name=""callback""/> to execute {methodDescription} the event type <typeparamref name=""TEvent""/> is raised.<br/>
        /// A weak reference to <paramref name=""handle""/> is stored. If the reference is garbage collected, the callback is not executed.
        /// </summary>
        /// <param name=""handle"">Object whose weak reference will be stored.</param>";

    public static string GetWeakUnsubscribeSummary(string methodReference) =>
        $@"/// <summary>
        /// Unsubscribes a callback suscribed by <see cref=""{methodReference}""/>.
        /// </summary>
        /// <param name=""handle"">Object whose weak reference was stored.</param>";
}
