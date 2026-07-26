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
    private string _receivedDataBuffer = string.Empty;
    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        _receivedDataBuffer += _serialPort.ReadExisting(); // Read all available data and append to buffer

        if (_receivedDataBuffer.Contains(_serialPort.NewLine)) // Check if the buffer contains a complete serial message (a communication)
        {
            bool bufferEndsWithNewLine = _receivedDataBuffer.EndsWith(_serialPort.NewLine);
            string[] messages = _receivedDataBuffer.Split(new string[] { _serialPort.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            
            if (!bufferEndsWithNewLine) 
            {
                _receivedDataBuffer = messages[^1]; // Keep the incomplete communication in the buffer
                Array.Resize(ref messages, messages.Length - 1); // Remove the communication message from the array
            }
            else
                _receivedDataBuffer = string.Empty; // Clear the buffer if it ends with a newline


            foreach (var message in messages) {
                OnCommunicationReceived?.Invoke(this, new ParsedCommunication(new ByteChain(message))); // Raise event for each complete communication
            }
        }
    }
}

