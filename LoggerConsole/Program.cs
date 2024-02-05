// See https://aka.ms/new-console-template for more information

using Logger;

Log.Information("Hello, World!");

Log.SetSeverity(Severity.Information);

// Log lots of useless data that will get ignored
for (int i = 0; i < 100; ++i) {
    Log.Verbose("This is a verbose message, it won't print anything");
    // Thread.Sleep(100);
}

Log.Information("Nothing was printed between the hello world and this");

