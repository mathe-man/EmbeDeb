namespace EmbeDebInterpreter.Message;

public abstract class Message
{
    protected void RaiseObjectCreated()
        => ObjectCreated?.Invoke(this, EventArgs.Empty);

    public static event EventHandler ObjectCreated;
}
