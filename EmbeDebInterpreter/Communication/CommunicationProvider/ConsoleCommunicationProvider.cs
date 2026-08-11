namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class ConsoleCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;

    
    public void ListenToConsoleInput()
    {
        while (true)
        {
            var read = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(read))
                continue;

            char[] input = read.ToArray();

            ByteChain result = new();

            for (int i = 0;; i++)
            {
                if (i >= input.Length)
                    break;

                // Number start are between { and }
                if (input[i] == '{')
                {
                    var endIndex = NextIndexOf('}', i, input);
                    
                    if (endIndex == -1)
                        continue;

                    var type = input[i + 1];
                    var numStr = read.Substring(i + 2, endIndex - i - 2);

                    // float parsing
                    if (type == 'f')
                    {
                        // TODO add correct culture to allow '.' instead of ',' in floating point
                        result.Add(float.Parse(numStr));
                        i = endIndex;
                    }
                    // int parsing
                    else if (type == 'i')
                    {
                        result.Add(Int32.Parse(numStr));
                        i = endIndex;
                    }

                }

                else
                    // Letters/chars
                    result.Add(input[i]);
            }

            var communication = new ParsedCommunication(result.Split('|'), "Console");

            OnCommunicationReceived?.Invoke(this, communication);
        }
    }

    private int NextIndexOf(char ch, int start, char[] array)
    {
        for (int i = start; i < array.Length; i++)
            if (array[i] == ch)
                return i;

        return -1;
    }
}
