using EmbeDebInterpreter.Communication;
using EmbeDebInterpreter.Communication.CommunicationProvider;
using EmbeDebInterpreter.Message;

namespace ExampleDebuggingProvider;

class Program
{
    static readonly string _headerMessage = """
        The DebuggingCommunicationProvider allow you to planify some communication directly from your code.
        A latency can be set as needed (wait for the console startup or to simulate a latency.

        This example will fire a communication every time you press Enter !
        And the latency will follow the Fibonacci sequence, starting at 0 and 0.1s that'll reset after 10 seconds.

        """;

    static void Main(string[] args)
    {
        // Presentation
        Console.WriteLine(_headerMessage);

        // == Interpreter classes construction ==

        DebuggingCommunicationProvider provider = new(0);

        // The lib already contain an handler that show 'Text' messages to the console
        MessageDispatcher dispatcher = new(true, provider);



        // Initial delays (latency) values
        (int prev, int current) delayMs = (100, 100);

        while (true)
        {
            Console.ReadLine();

            provider.SendCommunication($"XX|ExampleProgram|Txt=This message came with a delay of {delayMs.current}", delayMs.current);

            delayMs = (delayMs.current, delayMs.current + delayMs.prev);

            if (delayMs.current > 10_000)
                delayMs = (100, 100);
        }
    }
}