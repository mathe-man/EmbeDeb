using EmbeDebInterpreter.Communication;
using EmbeDebInterpreter.Communication.CommunicationProvider;
using System.Reflection;

namespace EmbeDebInterpreter.Message;

public class MessageDispatcher
{
    private MessageHandlerRegister _handlers = new();

    public MessageDispatcher(bool registerCurrentAssembly = true)
    {
        if (registerCurrentAssembly)
        {
            // Register handlers from the current assembly
            RegisterAssemblyHandlers(Assembly.GetExecutingAssembly());
        }
    }

    public MessageDispatcher(params ICommunicationProvider[] providers)
    {
        foreach (var provider in providers)
            SubscribeToProvider(provider);
    }

    public MessageDispatcher(bool registerCurrentAssembly, params ICommunicationProvider[] providers)
    {
        if (registerCurrentAssembly)
            // Register handlers from the current 
            RegisterAssemblyHandlers(Assembly.GetExecutingAssembly());
        

        // Subscribe to the provided communication providers
        foreach (var provider in providers)
            SubscribeToProvider(provider);
    }
    

    public void RegisterAssemblyHandlers(Assembly assembly)
    {
        var assemblyHandlers = assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Select(m => new {
                Method = m,
                Attr = m.GetCustomAttribute<MessageHandler>()
            })
            .Where(x => x.Attr != null)
            .ToDictionary(
                x => x.Attr.MessageId, // Key: IDs defined in the attribute
                x => x.Method          // Value: the method to call
            );


        foreach (var handler in assemblyHandlers)   // For each handler found in the assembly
            foreach (var messageId in handler.Key)  // For each message ID defined in the attribute of the handler  
                _handlers.AddHandler(messageId, handler.Value); // We add the handler for that message ID in the register
    }

    public void RegisterHandlers(params (string messageType, MethodInfo handler)[] handlersPairs)
    {
        foreach (var pair in handlersPairs)
            _handlers.AddHandler(pair.messageType, pair.handler);
    }

    public void RegisterHandler(string messageType, MethodInfo handler)
        => _handlers.AddHandler(messageType, handler);


    public int Dispatch(ParsedCommunication communication)
    {
        int result = 0; // We initialize a result variable to 0. This will be incremented for each message dispatched.
        
        foreach (var message in communication.Messages) // For each message in the communication
        {
            result += Dispatch(new RawMessage(message)); // We dispatch the message and get the result
        }

        return result;
    }
    public int Dispatch(RawMessage rawMessage)
        => _handlers.CallHandlers(rawMessage);

    public void SubscribeToProvider(ICommunicationProvider provider)
    {
        provider.OnCommunicationReceived += (_, communication) =>
        {
            Dispatch(communication);        
        };
    }
}