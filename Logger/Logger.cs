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
}

public abstract class Device
{
    public Severity MinimumSeverity { get; set; } = Severity.Verbose;

    protected Queue<Message> messages = new Queue<Message>();

    public void Log(Severity severity, string message)
    {
        if (severity < MinimumSeverity)
            return;
        messages.Enqueue(new Message { Severity = severity, Text = message });
        Notify();
    }

    protected abstract void Notify();
}

public class StreamDevice(TextWriter writer) : Device
{
    /// <summary>
    /// Specify the format string for the log message. The default is "{0} [{1}] {2}". Where
    /// {0} is the date, {1} is the severity, and {2} is the message.
    /// </summary>
    public string FormatStr { get; set; } = "{0} [{1}] {2}";

    private readonly TextWriter writer = writer;

    protected override void Notify()
    {
        while (messages.Count > 0)
        {
            var messageData = messages.Dequeue();
            var message = messageData.Text;
            var severity = messageData.Severity.ToString();
            var date = DateTime.Now;
            writer.WriteLine(string.Format(FormatStr, date, severity, message));
        }
    }
}

public class ConsoleDevice : StreamDevice
{
    public ConsoleDevice()
        : base(Console.Error) { }
}

public class Logger
{
    /// <summary>
    /// Build a logger with no devices. Will not write anything and will discard all messages.
    /// </summary>
    public Logger() { }

    /// <summary>
    /// Build a logger with the given devices.
    /// </summary>
    public Logger(IEnumerable<Device> devices) => this.devices.AddRange(devices);

    public void Verbose(string message) => Log(Severity.Verbose, message);

    public void Debug(string message) => Log(Severity.Debug, message);

    public void Information(string message) => Log(Severity.Information, message);

    public void Warning(string message) => Log(Severity.Warning, message);

    public void Error(string message) => Log(Severity.Error, message);

    public void Log(Severity severity, string message)
    {
        foreach (var device in devices)
        {
            device.Log(severity, message);
        }
    }

    public void AddDevice(Device device) => devices.Add(device);

    public static Logger GetConsoleLogger() => new([new ConsoleDevice()]);

    private readonly List<Device> devices = [];
}

public class Log
{
    public static void Verbose(string message) => LogGeneric(Severity.Verbose, message);

    public static void Debug(string message) => LogGeneric(Severity.Debug, message);

    public static void Information(string message) => LogGeneric(Severity.Information, message);

    public static void Warning(string message) => LogGeneric(Severity.Warning, message);

    public static void Error(string message) => LogGeneric(Severity.Error, message);

    public static void LogGeneric(Severity severity, string message)
    {
        Logger.Log(severity, message);
    }

    public static Logger Logger
    {
        get => m_logger;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            m_logger = value;
        }
    }

    private static Logger m_logger = Logger.GetConsoleLogger();
}
