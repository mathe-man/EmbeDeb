
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class TimeMessage<T> : Message
{
    public T Time { get; }
    public TimeMessage(T time)
    {
        Time = time;
        RaiseObjectCreated();
    }

    [MessageHandler("time", "time32")]
    public static Message Handle32(RawMessage me)
    {
        var time = UInt32.Parse(me.Content);
        return new TimeMessage<UInt32>(time);
    }

    [MessageHandler("time64")]
    public static Message Handle64(RawMessage me)
    {
        var time = UInt64.Parse(me.Content);
        return new TimeMessage<UInt64>(time);
    }

    [MessageHandler("time16")]
    public static Message Handle(RawMessage me)
    {
        var time = UInt16.Parse(me.Content);
        return new TimeMessage<UInt16>(time);
    }
}
