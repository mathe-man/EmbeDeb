
using System.Buffers.Binary;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;


namespace EmbeDebInterpreter;

public class ByteChain : IEnumerable<byte>
{
    private List<byte> _bytes = new ();

    
    public ByteChain() {}
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

    // [i..j] range operator
    public ByteChain this[Range range]
    {
        get => new(_bytes[range].ToList());
    }

    // Enumerator 
    public IEnumerator<byte> GetEnumerator() => _bytes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Insert(byte value, int index)
        => _bytes.Insert(index, value);

    public void Append(byte value)
        => _bytes.Append(value);


    public byte[] ToArray()
        => _bytes.ToArray();

    public ByteChain Get(int index, int count)
    {
        if (index + count > _bytes.Count)
            throw new IndexOutOfRangeException();

        ByteChain bytes = new ();

        for (int i = index; i < index + count; i++)
            bytes[i] = _bytes[i];

        return bytes;
    }

    public byte Get(int index)
        => _bytes[index];

    public string GetStr(int index, int count)
        => Encoding.UTF8.GetString(Get(index, count).ToArray());



    public UInt64 GetUInt64(int index)
        => BinaryPrimitives.ReadUInt64LittleEndian(Get(index, sizeof(UInt64)).ToArray());
    public UInt32 GetUInt32(int index)
        => BinaryPrimitives.ReadUInt32LittleEndian(Get(index, sizeof(UInt32)).ToArray());
    public UInt16 GetUInt16(int index)
        => BinaryPrimitives.ReadUInt16LittleEndian(Get(index, sizeof(UInt16)).ToArray());


    public Int64 GetInt64(int index)
        => BinaryPrimitives.ReadInt64LittleEndian(Get(index, sizeof(Int64)).ToArray());
    public Int32 GetInt32(int index)
        => BinaryPrimitives.ReadInt32LittleEndian(Get(index, sizeof(Int32)).ToArray());
    public Int16 GetInt16(int index)
        => BinaryPrimitives.ReadInt16LittleEndian(Get(index, sizeof(Int16)).ToArray());

    public float GetFloat(int index)
        => BinaryPrimitives.ReadSingleLittleEndian(Get(index, sizeof(float)).ToArray());
    public double GetDouble(int index)
        => BinaryPrimitives.ReadDoubleLittleEndian(Get(index, sizeof(double)).ToArray());



    public bool Contains(byte value)
        => _bytes.Contains(value);

    public bool Contains(char value)
        => IndexOf(value) != -1;

    public bool Contains(string value)
        => IndexOf(value) != -1;

    public bool Match(byte value, int index)
        => _bytes[index] == value;
    public bool Match(byte[] value, int index)
    {
        if (index + value.Length > _bytes.Count)
            return false;

        for (int i = 0; i < value.Length; i++)
        {
            if (_bytes[index + i] != value[i])
            {
                return false;
            }
        }

        return true;
    }


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
            if (Match(value, i))
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
            if (Match(value, i))
                last = i;
        }

        return last;
    }


    public bool StartWith(byte value)
        => _bytes[0] == value;
    public bool StartWith(char value)
        => StartWith(value.ToString());
    public bool StartWith(string value)
        => StartWith(Encoding.UTF8.GetBytes(value));

    public bool StartWith(byte[] value)
    {
        for (int i = 0; i < value.Length; i++)
            if (!Match(value[i], i))
                return false;

        return true;
    }

    
    public bool EndWith(byte value)
        => _bytes.Last() == value;
    public bool EndWith(char value)
        => EndWith(value.ToString());
    public bool EndWith(string value)
        => EndWith(Encoding.UTF8.GetBytes(value));

    public bool EndWith(byte[] value)
    {
        for (int i = _bytes.Count - 1; i >= 0; i--)
            if (!Match(value[i], i))
                return false;

        return true;
    }

    public ByteChain[] Split(byte[] separator)
    {
        List<ByteChain> elements = new List<ByteChain>();

        var chain = new ByteChain();

        for (int i=0; i < _bytes.Count;)
        {
            if (Match(separator, i))
            {
                i += separator.Length;

                if (chain.ToArray().Length > 0)
                { 
                    elements.Add(chain); 
                    chain = new ByteChain();
                }
            }

            else
            {
                chain.Append(_bytes[i++]);
            }

        }

        return elements.ToArray();
    }
}
