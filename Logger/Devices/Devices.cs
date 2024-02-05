// Ideally this would be multiple files, but for simplicity I'm putting them all in one file.

using System.Text;

namespace Logger;

public abstract class Device
{
    public Severity MinimumSeverity { get; set; } = Severity.Verbose;

    public async Task Log(Message message)
    {
        if (message.Severity < MinimumSeverity)
            return;
        await LogInternal(message);
    }

    public abstract Task LogInternal(Message message);
}

public class TextDevice(TextWriter writer) : Device
{
    /// <summary>
    /// Specify the format string for the log message. The default is "{0} [{1}] {2}". Where
    /// {0} is the date, {1} is the severity, and {2} is the message.
    /// </summary>
    public string FormatStr { get; set; } = "{0} [{1}] {2}";

    private readonly TextWriter writer = writer;

    public override async Task LogInternal(Message messageData)
    {
        var message = messageData.Text;
        var severity = messageData.Severity.ToString();
        var date = messageData.Date;

        // Simulate writing delay
        await Task.Delay(10);

        var toWrite = string.Format(FormatStr, date, severity, message);
        await writer.WriteLineAsync(toWrite);
    }
}

public class ConsoleDevice : TextDevice
{
    public ConsoleDevice()
        : base(Console.Error) { }
}

/// <summary>
/// A device that writes to a file.
///
/// <para>
/// If the file doesn't exist, it will be created but the directory
/// must exist. If the file exists, the logs will be appended to it.
/// </para>
/// </summary>
/// <param name="path">Path to the log file</param>
public class FileDevice(string path) : TextDevice(new StreamWriter(path, append: true)) { }

/// <summary>
/// Writes all the logs to a string. Useful for testing.
///
/// <para>
/// Retrieve the logs with <see cref="ToString"/>.
/// </para>
/// </summary>
public class StringDevice : Device
{
    public string FormatStr
    {
        get => textDevice.FormatStr;
        set => textDevice.FormatStr = value;
    }

    private readonly StringBuilder builder;

    private readonly TextDevice textDevice;

    public StringDevice()
    {
        builder = new StringBuilder();
        textDevice = new TextDevice(new StringWriter(builder));
    }

    public override async Task LogInternal(Message message)
    {
        await textDevice.LogInternal(message);
    }

    public override string ToString() => builder.ToString();

    public void Clear() => builder.Clear();
}

public class DatabaseDevice : Device
{
    public override async Task LogInternal(Message message)
    {
        // Simulate writing delay
        await Task.Delay(10);

        // Write here to the database
        // INSERT INTO Log (Date, Severity, Message)
        // VALUES (message.Date, message.Severity, message.Text);
    }
}
