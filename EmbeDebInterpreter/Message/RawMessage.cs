
namespace EmbeDebInterpreter.Message;

public class RawMessage
{
    public readonly string Type;
    public byte[] Time;
    public readonly string Content;
    public RawMessage(string type, string content)
    {
        Type = type;   
        Content = content;
    }
    public RawMessage(string source)
    {
        if (string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");

        if (!source.Contains('='))
        {
            // We get the index of the ',' before the time information
            var timeIndex = source.LastIndexOf(',');
            // TODO check exeptcion in case the string don't contain a comma

            Type = source.Substring(0, timeIndex);
            // TODO copy the time bytes



            Content = string.Empty;
            return;
        }
        
        var splitedSource = source.Split('=');

        Type = splitedSource[0];
        Content = splitedSource[1];
    }
}