using ColoryrServer.SDK;

namespace ColoryrTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}

[RobotIN(new int[] { })]
public class test
{
    public bool OnMessage(RobotMessage head)
    {
        return false; //true��ʾ�¼��Ѵ������
    }
    public bool OnMessagSend(RobotSend head)
    {
        return false;
    }
    public bool OnRobotEvent(RobotEvent head)
    {
        return false;
    }
}