namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public interface ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;
}
