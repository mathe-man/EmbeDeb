
using System.Reflection.Metadata;

namespace EmbeDebInterpreter.Message.MessageHandlers;

public interface ITelemetry
{
    public string Name { get; }
    public Type StoreType { get; }
}

public static class Telemetry
{
    public static List<ITelemetry> All = new List<ITelemetry>();
    public static ITelemetry? GetTelemetry(string name)
        => All.FirstOrDefault(t => t.Name == name);
}

public class Telemetry<T> : Message, ITelemetry
{
    public string Name { get; }
    public List<T> Samples { get; }
    public Type StoreType => typeof(T);


    public Telemetry(string name)
    {
        Name = name;
        Samples = new List<T>();

        Telemetry.All.Add(this);

        RaiseObjectCreated();
    }
    public void Add(T value)
        => Samples.Add(value);


    #region TypesHandling

    [MessageHandler("Telemetry", "telem")]
    public static Message Handle(RawMessage me)
    {
        // Float is the default type for telemetry, so we can just return a Telemetry<float> instance here.
        return new Telemetry<float>(me.Content);
    }

    [MessageHandler("TelemetryDouble", "telemdouble")]
    public static Message HandleDouble(RawMessage me)
    {
        // Return a Telemetry<double> instance for double telemetry.
        return new Telemetry<double>(me.Content);
    }

    [MessageHandler("TelemetryInt16", "telemint16")]
    public static Message HandleInt16(RawMessage me)
    {
        // Return a Telemetry<short> instance for 16-bit integer telemetry.
        return new Telemetry<short>(me.Content);
    }

    [MessageHandler("TelemetryInt32", "telemint32")]
    public static Message HandleInt32(RawMessage me)
    {
        // Return a Telemetry<int> instance for 32-bit integer telemetry.
        return new Telemetry<int>(me.Content);
    }

    [MessageHandler("TelemetryInt64", "telemint64")]
    public static Message HandleInt64(RawMessage me)
    {
        // Return a Telemetry<long> instance for 64-bit integer telemetry.
        return new Telemetry<long>(me.Content);
    }

    #endregion

    
}

public class Sample<T> : Message
{
    public T Value { get; }
    public Sample(T value)
    {
        Value = value;
        RaiseObjectCreated();
    }
}
// TODO Add sample handling