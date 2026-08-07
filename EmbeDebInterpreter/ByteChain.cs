
using System.Buffers.Binary;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;


namespace EmbeDebInterpreter;

public class ByteChain : IEnumerable<byte>
{
    private List<byte> _bytes = new ();
    private static Encoding asciiEncoding = Encoding.GetEncoding(
        "ASCII",
        EncoderFallback.ExceptionFallback,
        DecoderFallback.ExceptionFallback
        );
    
    public ByteChain() {}
    public ByteChain(byte[] bytes)
    {
        _bytes = bytes.ToList();
    }
    public ByteChain(List<byte> bytes)
    {
        _bytes = bytes;
    }
    public ByteChain(string ascii)
    {
        _bytes = GetBytesFromAscii(ascii).ToList();
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

    // String convertion
    public override string ToString()
        => GetAsciiFromBytes(_bytes.ToArray());

    // Byte array convertion
    public byte[] ToArray()
        => _bytes.ToArray();


    public int Count => _bytes.Count;

    private static byte[] GetBytesFromAscii(string text)
    {
        try
        {
            byte[] bytes = asciiEncoding.GetBytes(text);
            return bytes;
        }
        catch(EncoderFallbackException)
        {
            Console.WriteLine("The string must be composed by ascii character");
            return [0];
        }
    }
    private static string GetAsciiFromBytes(byte[] bytes)
    {
        try
        {
            return asciiEncoding.GetString(bytes);
        }
        catch(DecoderFallbackException)
        {
            Console.WriteLine("This byte array contains invalid ASCII characters");
            return string.Empty;
        }
    }

    public static byte[] GetBytesFor<T>(T value)
    {
        return value switch
        {
            byte b => [b],
            sbyte v => [(byte)v],

            short v => BitConverter.GetBytes(v),
            ushort v => BitConverter.GetBytes(v),

            int v => BitConverter.GetBytes(v),
            uint v => BitConverter.GetBytes(v),

            long v => BitConverter.GetBytes(v),
            ulong v => BitConverter.GetBytes(v),

            float v => BitConverter.GetBytes(v),
            double v => BitConverter.GetBytes(v),

            bool v => BitConverter.GetBytes(v),

            byte[] v => v,
            ByteChain v => v.ToArray(),

            char c => GetBytesFromAscii($"{c}"),
            string v => GetBytesFromAscii(v),

            _ => throw new NotSupportedException(
                $"Type '{typeof(T).Name}' is not supported.")
        };
    }



    public void Insert<T>(T value, int index)
    {
        var bytes = GetBytesFor(value);

        for (int i = 0; i < bytes.Length; i++)
            _bytes.Insert(index + i, bytes[i]);

    }
    
    public void Add<T>(T value) {
        foreach (byte b in GetBytesFor(value)) _bytes.Add(b);
    }

    public void RemoveAt(int index)
        => _bytes.RemoveAt(index);

    public void RemoveRange(int index, int count)
        => _bytes.RemoveRange(index, count);


    public ByteChain Get(int index, int count)
    {
        if (index + count > _bytes.Count)
            throw new IndexOutOfRangeException();

        ByteChain bytes = new ();

        for (int i = 0; i < count; i++)
            bytes.Add(_bytes[i + index]);

        return bytes;
    }
    public byte Get(int index)
        => _bytes[index];

    public string GetStr(int index, int count)
        => GetAsciiFromBytes(Get(index, count).ToArray());

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
        => GetSingle(index);
    public float GetSingle(int index)
        => BinaryPrimitives.ReadSingleLittleEndian(Get(index, sizeof(float)).ToArray());
    public double GetDouble(int index)
        => BinaryPrimitives.ReadDoubleLittleEndian(Get(index, sizeof(double)).ToArray());


    public bool Match<T>(T value, int index)
    {
        var bytes = GetBytesFor(value);

        if (index + bytes.Length > _bytes.Count)
            return false;

        for (int i = 0; i < bytes.Length; i++)
        {
            if (_bytes[index + i] != bytes[i])
            {
                return false;
            }
        }

        return true;
    }


    public int IndexOf<T>(T value)
    {
        var bytes = GetBytesFor(value);

        for (int i = 0; i <= _bytes.Count - bytes.Length; i++)
        {
            if (Match(value, i))
                return i;
        }

        return -1;
    }

    public int LastIndexOf<T>(T value)
    {
        var bytes = GetBytesFor(value);
        int last = -1;

        for (int i = 0; i <= _bytes.Count - bytes.Length; i++)
        {
            if (Match(value, i))
                last = i;
        }

        return last;
    }


    public bool StartWith<T>(T value)
    {
        var bytes = GetBytesFor(value);

        for (int i = 0; i < bytes.Length; i++)
            if (!Match(bytes[i], i))
                return false;

        return true;
    }

    public bool EndWith<T>(T value)
    {
        var bytes = GetBytesFor(value);

        for (int i = _bytes.Count - 1; i >= 0; i--)
            if (!Match(bytes[i], i))
                return false;

        return true;
    }


    public ByteChain[] Split<T>(T separator)
    {
        var bytesSeparator = GetBytesFor(separator);
        List<ByteChain> elements = new List<ByteChain>();

        // Init the first chain
        int chainIndex = 0;
        elements.Add(new ByteChain());

        for (int i=0; i < _bytes.Count;)
        {
            if (Match(separator, i))
            {
                i += bytesSeparator.Length;

                chainIndex++;
                elements.Add(new ByteChain());
            }

            else
            {
                elements[chainIndex].Add(_bytes[i++]);
            }

        }

        // Remove empty chains
        var empty = elements.Where(x => x.ToArray().Length == 0);

        for (int i = 0; i < empty.Count(); i++)
            elements.Remove(empty.ElementAt(i));

        return elements.ToArray();
    }
}
