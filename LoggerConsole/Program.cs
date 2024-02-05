// See https://aka.ms/new-console-template for more information

using Logger;

Log.Information("Hello, World!");

// Sets the severity on all the existing devices
Log.SetSeverity(Severity.Information);

Directory.CreateDirectory("out");

// This device will have all the outputs as the default severity of a device is Verbose
Log.AddDevice(new FileDevice("out/log.txt"));

// Log lots of useless data that will get ignored
for (int i = 0; i < 100; ++i) {
    Log.Verbose("This is a verbose message, it won't print anything");
}

Log.Information("Nothing was printed between the hello world and this");
Console.WriteLine("Waiting for the logger to finish processing messages");

Log.Logger.WaitUntilEmpty();


