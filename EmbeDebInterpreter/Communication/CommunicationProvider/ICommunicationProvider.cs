namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public interface ICommunicationProvider
{
    static readonly byte[] EmbedebMagicBytes = [0xEB, 0xDB];

    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;
}
