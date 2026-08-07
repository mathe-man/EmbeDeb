using System.Text;
using System.Buffers.Binary;


namespace EmbeDebInterpreter.Communication;
 
// TODO test the constructor and Build method of the class to ensure that they are working well

public class ParsedCommunication
{
    // First bytes of the communication
    public readonly ByteChain MagicNumber = new ByteChain(new List<byte>() { 0xEB, 0xDB });

    // Name of the board who sent the communication
    public readonly string BoardName;

    // Lenght of the original un-parsed communication
    public readonly  UInt64 Length;

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
    public ParsedCommunication(ByteChain[] messages, string boardName)
    {
        Messages = messages;
        BoardName= boardName;

        var length = boardName.Length + MagicNumber.Count;
        // Add the number of messages for the separators
        length += messages.Count() + 1;
        length += sizeof(UInt64);

        foreach (var message in messages)
            length += message.Count();

        Length = (ulong)length;
    }

    public ByteChain Build()
    {
        ByteChain build = new ByteChain();

        build.Add(MagicNumber);

        byte[] sizeBytes = new byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(sizeBytes, Length);

        build.Insert(sizeBytes, 2);


        build.Add(BoardName);
        build.Add('|');

        foreach (var message in Messages)
        {
            build.Add(message);
            build.Add('|');
        }

        
        return build;
    }
}