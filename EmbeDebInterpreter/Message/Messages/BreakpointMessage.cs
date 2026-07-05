
namespace EmbeDebInterpreter.Message.MessageHandlers;

public class BreakPointMessage : Message
{
    public DateTime ReceivedTime { get; }
    public BreakPointMessage(DateTime receivedTime)
    {
        ReceivedTime = receivedTime;
        RaiseObjectCreated();
    }
}

public class BreakpointMessageHandler {
    [MessageHandler("Breakpoint", "bp")]
    public static Message Handle(RawMessage me)
    {
        // You can add a breakpoint here for debugging purposes
        return new BreakPointMessage(DateTime.Now);
    }
}