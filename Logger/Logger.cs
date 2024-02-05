using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

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

    public async Task Log(Message message)
    {
        if (message.Severity < MinimumSeverity)
            return;
        await LogInternal(message);
    }

    protected abstract Task LogInternal(Message message);
}

public class TextDevice(TextWriter writer) : Device
{
    /// <summary>
    /// Specify the format string for the log message. The default is "{0} [{1}] {2}". Where
    /// {0} is the date, {1} is the severity, and {2} is the message.
    /// </summary>
    public string FormatStr { get; set; } = "{0} [{1}] {2}";

    private readonly TextWriter writer = writer;

    protected override async Task LogInternal(Message messageData)
    {
        var message = messageData.Text;
        var severity = messageData.Severity.ToString();
        var date = DateTime.Now;
        await writer.WriteLineAsync(string.Format(FormatStr, date, severity, message));
    }
}

public class ConsoleDevice : TextDevice
{
    public ConsoleDevice()
        : base(Console.Error) { }
}

public class FileDevice(string path) : TextDevice(new StreamWriter(path)) { }

public class Logger
{
    /// <summary>
    /// Build a logger with no devices. Will not write anything and will discard all messages.
    /// </summary>
    public Logger()
    {
        processThreadHandle = Task.Run(ProcessThread);
    }

    /// <summary>
    /// Build a logger with the given devices.
    /// </summary>
    public Logger(IEnumerable<Device> devices)
    {
        foreach (var device in devices)
        {
            AddDevice(device);
        }

        processThreadHandle = Task.Run(ProcessThread);
    }

    ~Logger()
    {
        processThreadHandle.Wait();
    }

    public readonly Dictionary<int, Device> Devices = [];

    private readonly BufferBlock<Message> messageQueue = new();

    private readonly Task processThreadHandle;

    private int nextDeviceId = 0;

    public static Logger GetConsoleLogger() => new([new ConsoleDevice()]);

    public void Verbose(string message) => Log(Severity.Verbose, message);

    public void Debug(string message) => Log(Severity.Debug, message);

    public void Information(string message) => Log(Severity.Information, message);

    public void Warning(string message) => Log(Severity.Warning, message);

    public void Error(string message) => Log(Severity.Error, message);

    public void Log(Severity severity, string message)
    {
        messageQueue.Post(new Message { Severity = severity, Text = message });
    }

    public int AddDevice(Device device)
    {
        var id = nextDeviceId++;
        Devices.Add(id, device);
        return id;
    }

    private async void ProcessThread()
    {
        Console.WriteLine("Logger process thread started");
        while (await messageQueue.OutputAvailableAsync())
        {
            var message = await messageQueue.ReceiveAsync();
            foreach (var (_, device) in Devices)
            {
                await device.Log(message);
            }
        }
        Console.WriteLine("Logger process thread stopped");
    }
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

    public static void SetSeverity(int deviceId, Severity severity)
    {
        Logger.Devices[deviceId].MinimumSeverity = severity;
    }

    public static void SetSeverity(Severity severity)
    {
        foreach (var (_, device) in Logger.Devices)
        {
            device.MinimumSeverity = severity;
        }
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
