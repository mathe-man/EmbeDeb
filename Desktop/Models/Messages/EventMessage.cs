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
    }

    [MessageHandler("event", "evt")]
    static public Message Handle(RawMessage me)
    {

        return new EventMessage(me.Content.ToString(), (uint)me.Time).Publish();
    }
}
