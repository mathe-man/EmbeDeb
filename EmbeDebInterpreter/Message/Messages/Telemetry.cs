
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

    [MessageHandler("sample", "spl")]
    public static Message? HandleSample(RawMessage me)
    {
        var split = me.Content.Split(',');

        var telemetry = GetTelemetry(split[0].ToString());
        if (telemetry == null)
            return null;

        Type t = telemetry.StoreType;
        ByteChain value = split[1];

        return t switch
        {
            _ when t == typeof(float)  => new Sample<float>(value.GetFloat(0), telemetry as Telemetry<float>),
            _ when t == typeof(double) => new Sample<double>(value.GetDouble(0), telemetry as Telemetry<double>),
            _ when t == typeof(short)  => new Sample<short>(value.GetInt16(0), telemetry as Telemetry<short>),
            _ when t == typeof(int)    => new Sample<int>(value.GetInt32(0), telemetry as Telemetry<int>),
            _ when t == typeof(long)   => new Sample<long>(value.GetInt64(0), telemetry as Telemetry<long>),
            _ => null
        };

    }


    #region TypesHandling

    [MessageHandler("Telemetry", "telem", "telemfloat", "telemsingle")]
    public static Message Handle(RawMessage me)
    {
        // Float is the default type for telemetry, so we can just return a Telemetry<float> instance here.
        return new Telemetry<float>(me.Content.ToString());
    }

    [MessageHandler("TelemetryDouble", "telemdouble")]
    public static Message HandleDouble(RawMessage me)
    {
        // Return a Telemetry<double> instance for double telemetry.
        return new Telemetry<double>(me.Content.ToString());
    }

    [MessageHandler("TelemetryInt16", "telemint16")]
    public static Message HandleInt16(RawMessage me)
    {
        // Return a Telemetry<short> instance for 16-bit integer telemetry.
        return new Telemetry<short>(me.Content.ToString());
    }

    [MessageHandler("TelemetryInt32", "telemint32")]
    public static Message HandleInt32(RawMessage me)
    {
        // Return a Telemetry<int> instance for 32-bit integer telemetry.
        return new Telemetry<int>(me.Content.ToString());
    }

    [MessageHandler("TelemetryInt64", "telemint64")]
    public static Message HandleInt64(RawMessage me)
    {
        // Return a Telemetry<long> instance for 64-bit integer telemetry.
        return new Telemetry<long>(me.Content.ToString());
    }

    #endregion
}

public class Telemetry<T> : Message, ITelemetry
{
    public string Name { get; }
    public List<Sample<T>> Samples { get; }
    public Type StoreType => typeof(T);


    public Telemetry(string name)
    {
        Name = name;
        Samples = new List<Sample<T>>();

        Telemetry.All.Add(this);

        RaiseObjectCreated();
    }
    public void Add(Sample<T> value)
        => Samples.Add(value);    
}

public interface ISample 
{
}
public class Sample<T> : Message, ISample
{
    public T Value { get; }
    public Telemetry<T> Telemetry { get; }
    public Sample(T value, Telemetry<T> telemetry)
    {
        Value = value;
        Telemetry = telemetry;
        telemetry.Add(this);
        RaiseObjectCreated();
    }
    
}
// TODO Add sample handling