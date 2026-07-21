
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class BasicTimeMessage : Message
{
    public int Time { get; }
    public BasicTimeMessage(int time)
    {
        Time = time;
        RaiseObjectCreated();
    }

    [MessageHandler("BasicTime", "bt")]
    public static Message Handle(RawMessage me)
    {
        var time = int.Parse(me.Content);
        return new BasicTimeMessage(time);
    }
}
