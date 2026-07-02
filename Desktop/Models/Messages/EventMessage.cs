using EmbeDebInterpreter.Message;

namespace Desktop.Models.Messages;

public class EventMessage : Message
{
    public readonly string EventName;
    public readonly uint ReportTime;

    static public List<EventMessage> GetEvents()
    {
        return _storage;
    }

    static private List<EventMessage> _storage = new();

    public EventMessage(string eventName, uint reportTime)
    {
        EventName = eventName;
        ReportTime = reportTime;

        _storage.Add(this);

        RaiseObjectCreated();
    }

    [MessageHandler("EventMessage", "e")]
    static public Message Handle(RawMessage me)
    {
        var args = me.Content.Split(',');

        EventMessage message = null;

        if (args.Length == 2 )
            message = new EventMessage(args[0], uint.Parse(args[1]));

        // Return a null value if the message is not valid
        return message;
    }
}
