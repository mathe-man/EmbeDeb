
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class SerieValueMessage : Message
{
    public uint Index { get; }
    public string Value { get; }
    public SerieValueMessage(uint index, string value)
    {
        Index = index;
        Value = value;

        RaiseObjectCreated();
    }

    [MessageHandler("SerieValue", "sv")]
    public static Message Handle(RawMessage me)
    {
        SerieValueMessage result
            = new SerieValueMessage(uint.Parse(me.Content.Split(',')[0]), me.Content.Split(',')[1]);
        return result;
    }
}