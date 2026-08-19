
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class StringMessage : Message
{
    public string Value { get; }
    public StringMessage(string value)
    {
        Value = value;
    }

    public override string ToString()
        => Value;

    [MessageHandler("String", "str")]
    public static Message Handle(RawMessage me)
    {
        var content = me.Content;
        return new StringMessage(content.ToString()).Publish();
    }
}