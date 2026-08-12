
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class TimeMessage<T> : Message
{
    public T Time { get; }
    public TimeMessage(T time)
    {
        Time = time;
        RaiseObjectCreated();
    }

    // Default
    [MessageHandler("time", "time32")]
    public static Message Handle32(RawMessage me)
    {
        var time = me.Content.GetUInt32(0);
        return new TimeMessage<UInt32>(time);
    }

    [MessageHandler("time64")]
    public static Message Handle64(RawMessage me)
    {
        var time = me.Content.GetUInt64(0);
        return new TimeMessage<UInt64>(time);
    }

    [MessageHandler("time16")]
    public static Message Handle(RawMessage me)
    {
        var time = me.Content.GetUInt16(0);
        return new TimeMessage<UInt16>(time);
    }
}
