
namespace EmbeDebInterpreter.Message;

public abstract class Message
{
    protected void RaiseObjectCreated()
        => ObjectCreated?.Invoke(this, EventArgs.Empty);


    // TODO make this static to be able to subscribe to the event without creating an instance of the class
    public event EventHandler ObjectCreated;
}


public class RawMessage
{
    public readonly string Type;
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
            Type = source;
            Content = string.Empty;
            return;
        }
        
        var splitedSource = source.Split('=');

        Type = splitedSource[0];
        Content = splitedSource[1];
    }
}