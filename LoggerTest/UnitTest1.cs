namespace LoggerTest;

using Logger;

[TestClass]
public class Basic
{
    [TestMethod]
    public void TestHelloWorld()
    {
        var logger = new Logger();
        logger.Log("Hello, World!");
    }
}