
using System.Text;
using System.Buffers.Binary;


namespace EmbeDebInterpreter;

public class ByteChain
{
    private List<byte> _bytes;

    public ByteChain(byte[] bytes)
    {
        _bytes = bytes.ToList();
    }
    public ByteChain(List<byte> bytes)
    {
        _bytes = bytes;
    }

    // [] operator
    public byte this[int index]
    {
        get => _bytes[index];
        set => _bytes[index] = value;
    }

    public void Insert(byte value, int index)
        => _bytes.Insert(index, value);

    public void Append(byte value)
        => _bytes.Append(value);


    public byte[] GetBytes()
        => _bytes.ToArray();

    public byte[] Get(int index, int count)
    {
        if (index + count > _bytes.Count)
            throw new IndexOutOfRangeException();

        byte[] bytes = new byte[count];

        for (int i = index; i < index + count; i++)
            bytes[i] = _bytes[i];

        return bytes;
    }

    public byte Get(int index)
        => _bytes[index];

    public string GetStr(int index, int count)
        => Encoding.UTF8.GetString(Get(index, count));



    public UInt64 GetUInt64(int index)
        => BinaryPrimitives.ReadUInt64LittleEndian(Get(index, sizeof(UInt64)));
    public UInt32 GetUInt32(int index)
        => BinaryPrimitives.ReadUInt32LittleEndian(Get(index, sizeof(UInt32)));
    public UInt16 GetUInt16(int index)
        => BinaryPrimitives.ReadUInt16LittleEndian(Get(index, sizeof(UInt16)));


    public Int64 GetInt64(int index)
        => BinaryPrimitives.ReadInt64LittleEndian(Get(index, sizeof(Int64)));
    public Int32 GetInt32(int index)
        => BinaryPrimitives.ReadInt32LittleEndian(Get(index, sizeof(Int32)));
    public Int16 GetInt16(int index)
        => BinaryPrimitives.ReadInt16LittleEndian(Get(index, sizeof(Int16)));

    public float GetFloat(int index)
        => BinaryPrimitives.ReadSingleLittleEndian(Get(index, sizeof(float)));
    public double GetDouble(int index)
        => BinaryPrimitives.ReadDoubleLittleEndian(Get(index, sizeof(double)));



    public bool Contains(byte value)
        => _bytes.Contains(value);

    public bool Contains(char value)
        => IndexOf(value) != -1;

    public bool Contains(string value)
        => IndexOf(value) != -1;


    public int IndexOf(byte value)
        => _bytes.IndexOf(value);

    public int IndexOf(char value)
        => IndexOf(value.ToString());

    public int IndexOf(string value)
        => IndexOf(Encoding.UTF8.GetBytes(value));

    public int IndexOf(byte[] value)
    {
        for (int i = 0; i <= _bytes.Count - value.Length; i++)
        {
            bool match = true;

            for (int j = 0; j < value.Length; j++)
            {
                if (_bytes[i + j] != value[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return i;
        }

        return -1;
    }

    public int LastIndexOf(byte value)
        => _bytes.LastIndexOf(value);

    public int LastIndexOf(char value)
        => LastIndexOf(value.ToString());

    public int LastIndexOf(string value)
        => LastIndexOf(Encoding.UTF8.GetBytes(value));

    public int LastIndexOf(byte[] value)
    {
        int last = -1;

        for (int i = 0; i <= _bytes.Count - value.Length; i++)
        {
            bool match = true;

            for (int j = 0; j < value.Length; j++)
            {
                if (_bytes[i + j] != value[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                last = i;
        }

        return last;
    }
}
