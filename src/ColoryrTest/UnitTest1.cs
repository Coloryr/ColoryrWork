using ColoryrServer.Core.Netty;

namespace ColoryrTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            NettyWebSocket.Start().Wait();
            Assert.Pass();
        }
    }
}