using System.IO.Ports;

namespace EmbeDebInterpreter.Communication.CommunicationProvider;

public class SerialCommunicationProvider : ICommunicationProvider
{
    public event EventHandler<ParsedCommunication>? OnCommunicationReceived;
    

    private SerialPort _serialPort;

    public SerialCommunicationProvider(string port, int baudRate, bool connectWithDTR)
    {
        _serialPort = new SerialPort(port, baudRate);
        _serialPort.NewLine = "\r\n";
        _serialPort.ReceivedBytesThreshold = 1; // Trigger DataReceived event when at least 1 byte is received
        _serialPort.DtrEnable = connectWithDTR; // Enable DTR if specified

        _serialPort.DataReceived += SerialPort_DataReceived; // Subscribe to the DataReceived event

        try
        {
            _serialPort.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening serial port: {ex.Message}");
        }
    }

    // Buffer to accumulate received data until a complete message is formed
    private ByteChain _chain = new();
    private readonly byte[] _buffer = new byte[256];

    private int currentCommunicationLength = 0;

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        while (_serialPort.BytesToRead > 0)
        {
            int count = _serialPort.Read(_buffer, 0, Math.Min(_buffer.Length, _serialPort.BytesToRead));

            for (int i = 0; i < count; i++)
                _chain.Add(_buffer[i]);



            // Check if the chain contain the Embedeb magic bytes
            int magicIndex = _chain.IndexOf(ICommunicationProvider.EmbedebMagicBytes);
            if (magicIndex > 0)
            {
                if (currentCommunicationLength > 0)
                {
                    if (_chain.ToArray().Length - magicIndex > currentCommunicationLength)
                    {
                        OnCommunicationReceived?.Invoke
                            (this, new ParsedCommunication(_chain.Get(magicIndex, currentCommunicationLength)));
                         
                        _chain.RemoveRange(magicIndex, currentCommunicationLength);
                        currentCommunicationLength = 0;
                    }
                }
                else
                {
                    var length = TryGettingCommunicationLenght(magicIndex);
                    if (length is not null)
                        currentCommunicationLength = (int)length;
                }
            }

            
        }
    }

    private uint? TryGettingCommunicationLenght(int magicBytesIndex)
    {
        var chainSize = _chain.ToArray().Length;

        // Count if the communication length as already been received
        // - MagicBytes (index and 1 more because it's 2 bytes)
        // - Size of an uint (4 bytes, uint32_t in C++)
        if (chainSize - magicBytesIndex - 1 - sizeof(uint) < 0)
            return null;

        return _chain.GetUInt32(magicBytesIndex + 2);
    }
}

