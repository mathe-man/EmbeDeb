
namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class DebuggingCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<string>? OnCommunicationReceived;

    public int defaultLatency;
    public DebuggingCommunicationProvider(int defaultLatencyMs = 0)
        => defaultLatency = defaultLatencyMs;

    public void SendCommunication(string message, int latencyMs = -1)
    {
        if (latencyMs < 0)
            latencyMs = defaultLatency;

        Thread.Sleep(latencyMs);

        OnCommunicationReceived?.Invoke(this, message);
    }

    // TODO: Create methods that take Message objects instead of strings then build the communication from this
}