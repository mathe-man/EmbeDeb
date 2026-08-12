
namespace EmbeDebInterpreter.Message;


public class RawMessage
{
    public readonly string Type;
    public ulong Time;
    public readonly ByteChain Content;
    public RawMessage(string type, ByteChain content)
    {
        Type = type;   
        Content = content;
    }
    public RawMessage(ByteChain source)
    {
        if (source == null) throw new ArgumentNullException("source");

        ByteChain header = source;
        // If there is a content separator then we split header and content, otherwise we admit that the source only consist of an header
        if (source.IndexOf("=") != -1)
        {
            header = source.Split("=")[0];
            Content = source.Split("=")[1];
        }
        else
            Content = new(); // Empty content

        // Index of the time information, last comma in the header
        var timeIndex = header.LastIndexOf(",");

        
        Time = timeIndex == -1 ? 0 : header.GetUInt64(timeIndex + 1);

        if (timeIndex == -1)
            Type = header.ToString();
        else
            Type = header.GetStr(0, header.Count - timeIndex);



        var splitedSource = source.Split('=');
    }
}