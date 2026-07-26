using System.Text;
using System.Buffers.Binary;


namespace EmbeDebInterpreter.Communication;
 
// TODO test the constructor and Build method of the class to ensure that they are working well

public class ParsedCommunication
{
    // First bytes of the communication
    public readonly ByteChain MagicNumber;

    // Name of the board who sent the communication
    public readonly string BoardName;

    // Lenght of the original un-parsed communication
    public readonly UInt64 Length;

    public readonly ByteChain[] Messages;

    public static readonly string MessageSeparator = "|";

    public ParsedCommunication(ByteChain source)
    {
        if (source == null) throw new ArgumentNullException("source");

        // Split the source by the message separator, the first part will be the header, and the rest will be the messages
        var splitedSource = source.Split(MessageSeparator);
        var header = splitedSource[0];

        // TODO handle possible errors (bad formated incoming communication)

        MagicNumber = header.Get(0, 2);
        Length = header.GetUInt64(2);

        int nameLength= header.ToArray().Length - 8 - 2; // -8 for the length and 2 for the magic bytes

        BoardName = header.GetStr(10, nameLength);


        Messages = new ByteChain[splitedSource.Length - 1];
        Array.Copy(splitedSource, 1, Messages, 0, Messages.Length); // Copy the messages from the splited source to the Messages array
    }

    public ByteChain Build()
    {
        ByteChain build = new ByteChain();

        build.Add(MagicNumber);

        // We'll insert the size at the end

        build.Add(BoardName);

        foreach (var message in Messages)
            build.Add(message);

        byte[] sizeBytes = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(sizeBytes, Length);

        build.Insert(sizeBytes, 2);

        return build;
    }
}