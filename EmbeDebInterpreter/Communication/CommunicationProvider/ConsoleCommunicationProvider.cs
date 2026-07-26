namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class ConsoleCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;
    
    public void ListenToConsoleInput()
    {
        while (true)
        {
            string input = Console.ReadLine() ?? string.Empty;
            OnCommunicationReceived?.Invoke(this, new ParsedCommunication(new ByteChain(input)));
        }
    }
}
