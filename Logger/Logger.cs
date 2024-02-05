using System.IO;
using System.Threading.Tasks.Dataflow;

namespace Logger;

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
        messageQueue.Post(
            new Message
            {
                Severity = severity,
                Text = message,
                Date = DateTime.Now
            }
        );
    }

    public int AddDevice(Device device)
    {
        // This is not thread safe, in a more elaborate setup you can add a mutex here and
        // also the ProcessThread when reading the devices.
        var id = nextDeviceId++;
        Devices.Add(id, device);
        return id;
    }

    /// <summary>
    /// Wait until the message queue is empty and all the messages have been processed.
    /// </summary>
    public void WaitUntilEmpty()
    {
        messageQueue.Complete();
        processThreadHandle.Wait();
    }

    private async Task ProcessThread()
    {
        while (await messageQueue.OutputAvailableAsync() || messageQueue.Count > 0)
        {
            var message = await messageQueue.ReceiveAsync();
            foreach (var (_, device) in Devices)
            {
                // Send the message to the device that will handle it. 
                // This can be a file, console, database, etc.
                await device.Log(message);
            }
        }
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

    public static void AddDevice(Device device)
    {
        Logger.AddDevice(device);
    }

    public static void WaitUntilEmpty()
    {
        Logger.WaitUntilEmpty();
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
