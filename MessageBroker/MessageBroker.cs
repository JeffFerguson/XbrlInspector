namespace XbrlInspector.MessageBroker
{
    /// <summary>
    /// A broker for handling messages sent between pages. Sending messages between view
    /// models allows pages to coordinate work without requiring that the pages have
    /// explicit object references between them.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To send messages between pages, you will first need to define a class that represents
    /// the message. This class should be available from the XbrlInspector.Messages namespsace.
    /// The class does not need to be derived from a base class nor does it need to implement an
    /// interface. The class can be defined with constructor parameters, properties, or methods,
    /// approprite to the message.
    /// </para>
    /// <para>
    /// The message broker is used to publish messages and to subscribe to the messages. It is intended
    /// to be a singleton class accessed through the public static property called Instance. The
    /// expression MessageBroker.Instance should be used to get a reference to the singleton instance.
    /// </para>
    /// <para>
    /// To publish a message from a view model, use the Publish() method from the message broker's
    /// static instance, as in the following example:
    /// </para>
    /// <code>
    /// MessageBroker.MessageBroker.Instance.Publish&lt;[MessageClass]&gt;(this, new [MessageClass]());
    /// </code>
    /// <para>
    /// Pages that want to listen for a message must call Subscribe() from the static broker
    /// instance, passing an Action parameter that handles the message whenever it arrives, as in the
    /// following example:
    /// </para>
    /// <code>
    /// MessageBroker.MessageBroker.Instance.Subscribe&lt;[MessageClass]&gt;(MessageHandler);
    /// ...
    /// private void MessageHandler(MessagePayload&lt;[MessageClass]&gt; payload)
    /// {
    ///     var message = payload.What;
    /// }
    /// </code>
    /// </remarks>
    public class MessageBroker : IMessageBroker
    {
        private static MessageBroker _instance;
        private readonly Dictionary<Type, List<Delegate>> _subscribers;

        public static MessageBroker Instance => MessageBroker._instance;

        static MessageBroker()
        {
            _instance = new MessageBroker();
        }

        public MessageBroker()
        {
            _subscribers = new Dictionary<Type, List<Delegate>>();
        }

        public void Publish<T>(object source, T message)
        {
            if (message == null || source == null)
                return;
            if (!_subscribers.ContainsKey(typeof(T)))
            {
                return;
            }
            var delegates = _subscribers[typeof(T)];
            if (delegates == null || delegates.Count == 0) return;
            var payload = new MessagePayload<T>(message, source);
            foreach (var handler in delegates.Select
            (item => item as Action<MessagePayload<T>>))
            {
                Task.Factory.StartNew(() => handler?.Invoke(payload));
            }
        }

        public void Subscribe<T>(Action<MessagePayload<T>> subscription)
        {
            var delegates = _subscribers.ContainsKey(typeof(T)) ?
                            _subscribers[typeof(T)] : new List<Delegate>();
            if (!delegates.Contains(subscription))
            {
                delegates.Add(subscription);
            }
            _subscribers[typeof(T)] = delegates;
        }

        public void Unsubscribe<T>(Action<MessagePayload<T>> subscription)
        {
            if (!_subscribers.ContainsKey(typeof(T))) return;
            var delegates = _subscribers[typeof(T)];
            if (delegates.Contains(subscription))
                delegates.Remove(subscription);
            if (delegates.Count == 0)
                _subscribers.Remove(typeof(T));
        }

        public void Dispose()
        {
            _subscribers?.Clear();
        }
    }
}
