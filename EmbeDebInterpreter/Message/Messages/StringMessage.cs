
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class StringMessage : Message
{
    public string Value { get; }
    public StringMessage(string value)
    {
        Value = value;
        RaiseObjectCreated();
    }

    public override string ToString()
        => Value;
}

public class StringMessageHandler
{
    [MessageHandler("String", "str")]
    public static Message Handle(RawMessage me)
    {
        Console.WriteLine($"StringMessageHandler: {me.Content}");
        return new StringMessage(me.Content);
    }
}