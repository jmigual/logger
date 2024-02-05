namespace LoggerTest;

using System.Text;
using Logger;

[TestClass]
public class Basic
{
    [TestMethod]
    public void TestHelloWorld()
    {
        Log.Information("Hello, World!");
    }

    [TestMethod]
    public void TestFormatStr()
    {
        var device = new StringDevice
        {
            FormatStr = "{1} {2}"
        };

        var logger = new Logger([device]);
        logger.Information("Hello, World!");
        Task.Delay(100).Wait(); // Wait for logger to write to the device

        var expected = "Information Hello, World!" + Environment.NewLine;
        Assert.AreEqual(expected, device.ToString());

        device.FormatStr = "{0:yyyy} {1} {2}";
        device.Clear();
        var date = DateTime.Now; // This can potentially fail if run in new-year's eve
        logger.Information("Hello, World!");
        Task.Delay(100).Wait(); // Wait for logger to write to the device

        expected = $"{date:yyyy} Information Hello, World!" + Environment.NewLine;
        Assert.AreEqual(expected, device.ToString());
    }
}