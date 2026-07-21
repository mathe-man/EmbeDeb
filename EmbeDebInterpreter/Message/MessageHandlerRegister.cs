using System.Reflection;

namespace EmbeDebInterpreter.Message;

internal class MessageHandlerRegister
{
    Dictionary<string, List<MethodInfo>> _handlers = new();

    public void AddHandler(string messageType, MethodInfo handler)
    {
        if (!_handlers.ContainsKey(messageType))
            _handlers[messageType] = new List<MethodInfo>();
        _handlers[messageType].Add(handler);
    }

    public void RemoveHandler(string messageType, MethodInfo handler)
    {
        if (_handlers.ContainsKey(messageType))
            _handlers[messageType].Remove(handler);

        if (_handlers.ContainsKey(messageType) && _handlers[messageType].Count == 0)
            _handlers.Remove(messageType);
    }

    public int CallHandlers(RawMessage message)
    {
        int returnValue = 0;    // We initialize a return value to 0. This will be incremented for each handler found for the message type.

        foreach (var key in _handlers.Keys)     // For each key in the handlers dictionary
            if (message.Type.Contains(key))     // If the message type contains the key (so the message type can target multiple handlers)
                foreach (var handler in _handlers[key])     // For each handler associated with that key
                {
                    handler.Invoke(null, new object[] { message }); // We invoke the handler, passing the message as an argument. The first argument is null because we are calling a static method.
                    returnValue++; // We increment the return value for each handler found for the message type.
                }

        return returnValue;
    }
}

