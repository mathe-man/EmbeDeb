
namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class DebuggingCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;

    public int defaultLatency;
    public DebuggingCommunicationProvider(int defaultLatencyMs = 0)
        => defaultLatency = defaultLatencyMs;

    private void ApplyLatency(int latencyMs)
    {
        if (latencyMs < 0)
            latencyMs = defaultLatency;

        Thread.Sleep(latencyMs);
    }

    public void SendCommunication(string message, int latencyMs = -1)
    {
        ApplyLatency(latencyMs);

        OnCommunicationReceived?.Invoke(this, new ParsedCommunication(new ByteChain(message)));
    }

    public void SendCommunication(ByteChain chain, int latencyMs = -1)
    {
        ApplyLatency(latencyMs);

        OnCommunicationReceived?.Invoke(this, new ParsedCommunication(chain));
    }

    public void SendCommunication(ParsedCommunication communication, int latencyMs = -1)
    {
        ApplyLatency(latencyMs);
        
        OnCommunicationReceived?.Invoke(this, communication);
    }


    // TODO: Create methods that take Message objects instead of strings then build the communication from this
}