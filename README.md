# logger
A small logging library

## Usage

To use this library you can just use the `Log` class, which contains a default logger that logs to the standard error. You can also create your own logger and add devices to it.

```csharp
using Logger;

Log.Information("Hello, World!");

// Sets the severity on all the existing devices
Log.SetSeverity(Severity.Information);

Directory.CreateDirectory("out");

// This device will have all the outputs as the default severity of a device is Verbose
Log.AddDevice(new FileDevice("out/log.txt"));

Log.Verbose("This will be written to the file but not to the console");
```

### Configuring the output.

The text devices support a `.Format` property to configure the output. The default format is `"{0} [{1}] {2}"`. Where `{0}` is the timestamp, `{1}` is the severity and `{2}` is the message.

```csharp
var console = new ConsoleDevice();
console.Format = "{0} {1}: {2}";

Log.AddDevice(console);
Log.Information("Hello, World!");

// Possible output:
// "2021-08-15 12:00:00 Information: Hello, World!"
```


### Logging levels

The library contains the following logging levels:
- `Verbose`: For debugging purposes.
- `Debug`: For debugging purposes.
- `Information`: For general information.
- `Warning`: For non-critical errors.
- `Error`: For critical errors.
- `Fatal`: For fatal errors.

## Console example

You can run a small example located in `LoggerConsole/Program.cs` with dotnet:

```bash
dotnet run --project LoggerConsole
```

## Code structure

The library is located in the `Logger` folder. It contains the following classes:
- `Logger`: The main class, which is used to log messages.
- `Log`: A static class with a default logger that logs to the standard error.
- `Message`: A class that represents a log message with a severity, the log message and a timestamp.
- `Device`: Each device writes a message to a specific output. The library contains the following devices: 
  - `ConsoleDevice`: To write to a console.
  - `FileDevice`: To write to a file.
  - `StringDevice`: To write to a string.
  - `DatabaseDevice`: To write to a database. Not implemented, just a stub.