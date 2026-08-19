namespace EmbeDebInterpreter.Message;

public abstract class Message
{
    private static readonly Dictionary<Type, List<Action<Message>>> _handlers = new ();

    public static void OnMessage<T>(Action<T> handler) where T : Message
    {
        var type = typeof(T);

        if (!_handlers.TryGetValue(type, out var handlers))
        {
            handlers = new List<Action<Message>>();
            _handlers.Add(type, handlers);
        }

        handlers.Add(msg => handler((T)msg));
    }





    public Message Publish()
    {
        var type = GetType();

        if (!_handlers.TryGetValue(type, out var handlers))
            return this;

        foreach (var handler in handlers)
            handler(this);

        return this;
    }
}
