namespace Logger;

public enum Severity
{
    Verbose,
    Debug,
    Information,
    Warning,
    Error,
    Fatal
}

public struct Message
{
    public Severity Severity;
    public string Text;

    public DateTime Date;
}
