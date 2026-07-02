using EmbeDebInterpreter.Message;
using System.Globalization;

namespace Desktop.Models.Messages;

public class TimeSerieValueMessage : Message
{   
    public readonly string SerieName;
    public readonly uint TimePoint;
    public readonly float Value;
    public TimeSerieValueMessage(string serieName, uint timePoint, float value)
    {
        SerieName = serieName;
        TimePoint = timePoint;
        Value = value;
        RaiseObjectCreated();
    }
    

    static public List<string> GetSeriesNames() {
        return _storage.Keys.ToList();
    }

    static public List<TimeSerieValueMessage> GetSerie(string serieName)
    {
        if (_storage.Keys.Contains(serieName))
            return _storage[serieName];
        else
            return new List<TimeSerieValueMessage>();
    }


    static private Dictionary<string, List<TimeSerieValueMessage>> _storage = new();

    [MessageHandler("TimeSerieValue", "tsv")]
    static public Message Handle(RawMessage me)
    {
        var args = me.Content.Split(',');

        // The message should contain at least 3 arguments: the name of the time series, the time point, and the value.
        if (args.Length < 3) return null;
        
        var message = new TimeSerieValueMessage(
            args[0],
            uint.Parse(args[1]),
            float.Parse(args[2], CultureInfo.InvariantCulture));

        // If the time series already exists, add the new value to it. Otherwise, create a new time series with the given name and add the value to it.
        if (_storage.Keys.Contains(message.SerieName))
            _storage[args[0]].Add(message);

        else
            _storage.Add(message.SerieName, new List<TimeSerieValueMessage> { message });


        return message;
    }
}
