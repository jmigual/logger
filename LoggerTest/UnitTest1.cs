namespace LoggerTest;

using Logger;

[TestClass]
public class Basic
{
    [TestMethod]
    public void TestHelloWorld()
    {
        Log.Information("Hello, World!");
    }
}