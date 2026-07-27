namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class ConsoleCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;
    
    public void ListenToConsoleInput()
    {
        while (true)
        {
            ByteChain input = new (Console.ReadLine() ?? string.Empty);

            OnCommunicationReceived?.Invoke(this, new ParsedCommunication(input));
        }
    }
}
