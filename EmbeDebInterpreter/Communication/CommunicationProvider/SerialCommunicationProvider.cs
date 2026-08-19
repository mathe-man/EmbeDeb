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
            Console.WriteLine($"Serial port {port} opened at {baudRate} baud.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening serial port: {ex.Message}");
        }
    }

    // Buffer to accumulate received data until a complete message is formed
    private ByteChain _chain = new();
    private readonly byte[] _buffer = new byte[256];

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        Console.WriteLine($"Data received on serial port: {_serialPort.BytesToRead} bytes left");
        
        while (_serialPort.BytesToRead > 0)
        {
            int count = _serialPort.Read(
                _buffer,
                0,
                Math.Min(_buffer.Length, _serialPort.BytesToRead)
            );

            for (int i = 0; i < count; i++)
                _chain.Add(_buffer[i]);

            ProcessChain();
        }
    }

    private void ProcessChain()
    {
        while (true)
        {
            int magicIndex = _chain.IndexOf(ICommunicationProvider.EmbedebMagicBytes);

            if (magicIndex < 0)
                return;

            // Remove bytes before the magic bytes
            if (magicIndex > 0)
            {
                _chain.RemoveRange(0, magicIndex);
                magicIndex = 0;
            }

            // To small to actually read the communication length
            if (_chain.ToArray().Length < 2 + sizeof(uint))
                return;

            uint length = _chain.GetUInt32(2);

            // Incomplete communication
            if (_chain.ToArray().Length < length)
                return;

            // Complete communication
            var communication = _chain.Get(0, (int)length);

            OnCommunicationReceived?.Invoke(
                this,
                new ParsedCommunication(communication)
            );

            // Remove transmited communication
            _chain.RemoveRange(0, (int)length);
        }
    }
}

